using Cryptopia.Admin.Common.Nzdt;
using Cryptopia.Common.DataContext;
using Cryptopia.Entity;
using Cryptopia.Enums;
using Cryptopia.Infrastructure.Common.DataContext;
using Cryptopia.Infrastructure.Common.Results;
using LumenWorks.Framework.IO.Csv;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Cryptopia.Admin.Core.Nzdt
{
	public class NzdtImportService : INzdtImportService
	{
		public IExchangeDataContextFactory ExchangeDataContextFactory { get; set; }
		public IDataContextFactory HubDataContextFactory { get; set; }

		private static Dictionary<string, bool> AllowedTranTypeDictionary = new Dictionary<string, bool>() {
			{ "CREDIT", true},
			{ "D/C", true},
			{ "TFR IN", true},

			{ "BANK FEE", false},
			{ "INT", false},
			{ "DEBIT", false},
			{ "TFR OUT", false},
		};

		public async Task<IServiceResult<NzdtUploadResultModel>> ValidateAndUpload(string adminUserId, Stream inputStream)
		{
			var processResult = await ProcessCsv(inputStream);
			if (!processResult.Success || !processResult.HasResult)
			{
				return new ServiceResult<NzdtUploadResultModel>(false, processResult.Message);
			}

			var csvTransactionItems = processResult.Result;
			var firstDate = csvTransactionItems.Min(x => x.Date);

			var resultModel = new NzdtUploadResultModel
			{
				TotalCount = csvTransactionItems.Count
			};

			List<NzdtTransaction> transactionsToUpload = new List<NzdtTransaction>();

			long firstUploadedTransactionUniqueId = 0;
			long lastUploadedTransactionUniqueId = 0;

			using (var context = ExchangeDataContextFactory.CreateContext())
			{
				var transactions = await context.NzdtTransaction
					.Where(t => t.Date >= firstDate)
					.ToListNoLockAsync().ConfigureAwait(false);

				var addressProjections = await context.Address
					.Where(a => a.CurrencyId == Constant.NZDT_ID)
					.Select(a => new AddressProjection
					{
						UserId = a.UserId,
						UserName = a.User.UserName,
						VerificationLevel = a.User.VerificationLevel,
						AddressHash = a.AddressHash,
					}).ToListNoLockAsync().ConfigureAwait(false);

				foreach (var item in csvTransactionItems)
				{
					var matchingTransaction = transactions.FirstOrDefault(x => x.UniqueId == item.UniqueId);
					var matchingAddress = addressProjections.FirstOrDefault(x => item.Memo.ToUpper().Contains(x.Address));

					if (matchingTransaction != null)
					{
						//Transaction Exists
						resultModel.ExistingCount++;
						continue;
					}

					if (matchingAddress != null)
					{
						var isVerificationValid = (matchingAddress.VerificationLevel == VerificationLevel.Level2 || matchingAddress.VerificationLevel == VerificationLevel.Level3);

						if (isVerificationValid)
						{
							transactionsToUpload.Add(new NzdtTransaction
							{
								TransactionStatus = NzdtTransactionStatus.ReadyForProcessing,
								UserId = matchingAddress.UserId,

								Date = item.Date,
								UniqueId = item.UniqueId,
								TranType = item.TranType,
								ChequeNumber = item.ChequeNumber,
								Payee = item.Payee,
								Memo = item.Memo,
								Amount = item.Amount,

								CreatedOn = DateTime.UtcNow
							});

							resultModel.ReadyForProcessingCount++;
						}
						else
						{
							transactionsToUpload.Add(new NzdtTransaction
							{
								TransactionStatus = NzdtTransactionStatus.ErrorUserNotVerified,
								UserId = matchingAddress.UserId,

								Date = item.Date,
								UniqueId = item.UniqueId,
								TranType = item.TranType,
								ChequeNumber = item.ChequeNumber,
								Payee = item.Payee,
								Memo = item.Memo,
								Amount = item.Amount,

								CreatedOn = DateTime.UtcNow
							});

							resultModel.ErroredCount++;
						}
					}
					else
					{
						transactionsToUpload.Add(new NzdtTransaction
						{
							TransactionStatus = NzdtTransactionStatus.ErrorUserNotFound,

							Date = item.Date,
							UniqueId = item.UniqueId,
							TranType = item.TranType,
							ChequeNumber = item.ChequeNumber,
							Payee = item.Payee,
							Memo = item.Memo,
							Amount = item.Amount,

							CreatedOn = DateTime.UtcNow
						});

						resultModel.ErroredCount++;
					}
				}

				if (resultModel.TotalCount != (resultModel.ExistingCount + resultModel.ReadyForProcessingCount + resultModel.ErroredCount))
				{
					return new ServiceResult<NzdtUploadResultModel>(false, "Error. Processed Transaction Count was out from Total Count");
				}

				if (transactionsToUpload.Any())
				{
					firstUploadedTransactionUniqueId = transactionsToUpload.First().UniqueId;
					lastUploadedTransactionUniqueId = transactionsToUpload.Last().UniqueId;
				}

				context.NzdtTransaction.AddRange(transactionsToUpload);
				await context.SaveChangesAsync().ConfigureAwait(false);
			}

			using (var context = HubDataContextFactory.CreateContext())
			{
				var logMessage = $"[NZDT Import] Imported {resultModel.ReadyForProcessingCount}. ";

				if (resultModel.ReadyForProcessingCount > 0)
				{
					logMessage += $"UniqueIds between {firstUploadedTransactionUniqueId} and {lastUploadedTransactionUniqueId}";
				}

				context.LogActivity(adminUserId, logMessage);
				await context.SaveChangesAsync().ConfigureAwait(false);
			}

			return new ServiceResult<NzdtUploadResultModel>(true, resultModel);
		}

		private async Task<IServiceResult<List<CsvTransactionItem>>> ProcessCsv(Stream inputStream)
		{
			try
			{
				List<string> csvLines = new List<string>();
				using (StreamReader reader = new StreamReader(inputStream))
				{
					string line;
					while ((line = await reader.ReadLineAsync().ConfigureAwait(false)) != null)
					{
						csvLines.Add(line);
					}
				}

				var details = csvLines.Take(6).ToList();
				if (details.Any(d => d == string.Empty))
				{
					return new ServiceResult<List<CsvTransactionItem>>(false, "ASB Detail Headers are Missing");
				}

				var columnHeadings = csvLines[6].Split(',').ToList();
				if (columnHeadings.Any(d => d == string.Empty))
				{
					return new ServiceResult<List<CsvTransactionItem>>(false, "Column Headings do not fit expected format");
				}

				//csvLines[7] should be empty

				var csvItemRows = csvLines.Skip(8).ToList();
				if (csvItemRows.Count == 0)
				{
					return new ServiceResult<List<CsvTransactionItem>>(false, "No Transactions Found");
				}

				List<CsvTransactionItem> csvItems = new List<CsvTransactionItem>();

				for (int i = 0; i < csvItemRows.Count; i++)
				{
					var line = csvItemRows[i];
					var processResult = ProcessCsvRow(line);

					if (processResult.Success)
					{
						var item = processResult.Result;

						var isAmountPositive = item.Amount > 0;
						var isAllowedTranType = AllowedTranTypeDictionary[item.TranType];

						if (isAmountPositive & isAllowedTranType)
						{
							csvItems.Add(item);
						}

						continue;
					}
					else
					{
						return new ServiceResult<List<CsvTransactionItem>>(false, $"Row {i} Failed with error: {processResult.Message}");
					}
				}

				return new ServiceResult<List<CsvTransactionItem>>(true, csvItems);
			}
			catch (Exception ex)
			{
				return new ServiceResult<List<CsvTransactionItem>>(false, $"Exception: {ex.GetType().Name} thrown when processing CSV");
			}
		}

		private static IServiceResult<CsvTransactionItem> ProcessCsvRow(string line)
		{
			try
			{
				var columns = new List<string>();
				using (var csv = new CsvReader(new StringReader(line), false))
				{
					var fieldCount = csv.FieldCount;
					while (csv.ReadNextRecord())
					{
						for (int i = 0; i < fieldCount; i++)
						{
							columns.Add(csv[i]);
						}
					}
				}

				if (columns.Count != 7)
				{
					return new ServiceResult<CsvTransactionItem>(false, "Column length does not equal 7");
				}

				DateTime date;
				var dateResult = DateTime.TryParse(columns[0], out date);
				if (!dateResult)
				{
					return new ServiceResult<CsvTransactionItem>(false, "Parsing row Date Failed");
				}

				//Convert DateTime from NZST to UTC
				var nzTimeInfo = TimeZoneInfo.FindSystemTimeZoneById("New Zealand Standard Time");
				var utcDate = TimeZoneInfo.ConvertTimeToUtc(date, nzTimeInfo);

				long uniqueId;
				var uniqueIdResult = long.TryParse(columns[1], out uniqueId);
				if (!uniqueIdResult)
				{
					return new ServiceResult<CsvTransactionItem>(false, "Parsing row UniqueId Failed");
				}

				decimal amount;
				var amountResult = decimal.TryParse(columns[6], out amount);
				if (!amountResult)
				{
					return new ServiceResult<CsvTransactionItem>(false, "Parsing row Amount Failed");
				}

				var model = new CsvTransactionItem
				{
					UniqueId = uniqueId,
					Date = utcDate,

					TranType = columns[2],
					ChequeNumber = columns[3],
					Payee = columns[4],
					Memo = columns[5],

					Amount = amount
				};

				if (!AllowedTranTypeDictionary.ContainsKey(model.TranType))
				{
					return new ServiceResult<CsvTransactionItem>(false, "Unknown TranType");
				}

				return new ServiceResult<CsvTransactionItem>(true, model);
			}
			catch (Exception ex)
			{
				return new ServiceResult<CsvTransactionItem>(false, $"Exception: {ex.GetType().Name} thrown");
			}
		}

		private class CsvTransactionItem
		{
			public long UniqueId { get; set; }
			public DateTime Date { get; set; }
			public string TranType { get; set; }
			public string ChequeNumber { get; set; }
			public string Payee { get; set; }
			public string Memo { get; set; }
			public decimal Amount { get; set; }
		}

		private class AddressProjection
		{
			public Guid UserId { get; set; }
			public string UserName { get; set; }
			public VerificationLevel VerificationLevel { get; set; }
			public string AddressHash { get; set; }
			public string Address => string.Concat(this.AddressHash.Skip(3).Take(8)).ToUpper();
		}

	}
}
