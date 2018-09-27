using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

using Moq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Cryptopia.Entity;
using Cryptopia.IntegrationService.Implementation;
using System.Threading;
using IntegrationServiceTests.DataContext;
using System.Threading.Tasks;
using System.Data.Entity.Infrastructure;

namespace IntegrationServiceTests
{
	[TestClass]
	public class CEFSProcessorTests
	{
		const int MAX_USER_COUNT = 100;
		const decimal TOTAL_PORTIONS = 6300;
		const decimal FOUR_POINT_FIVE_PERCENT = 0.045M;

		readonly DateTime _testMonth = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month - 1, 1);

		CEFSProcessor _processor;

		Mock_ExchangeDataContextFactory _exchangeContextFactory;
		Mock_DataContextFactory _dataContextFactory;

		IQueryable<User> _users;
		IQueryable<TradeHistory> _tradeHistories;
		IQueryable<TradePair> _tradePairs;
		IQueryable<Currency> _currencies;
		IQueryable<PaytopiaItem> _paytopiaPaymentItems;
		IQueryable<PaytopiaPayment> _paytopiaPayments;

		Dictionary<int, decimal> _tradeHistoryFeeByCurrency;
		Dictionary<int, decimal> _paytopiaPaymentTotalByCurrency;
		Dictionary<int, decimal> _tradeHistoryFeePortionsByCurrency;
		Dictionary<int, decimal> _paytopiaPaymentPortionsByCurrency;

		Dictionary<int, decimal> _expectedFeeTotalsByCurrency;
		Dictionary<int, decimal> _expectedPortionOfCurrency;
		Dictionary<int, decimal> _expectedPaytopiaTotalsByCurrency;
		Dictionary<int, decimal> _expectedPaytopiaPortionOfCurrency;

		[TestInitialize]
		public void Initialize()
		{
			_expectedFeeTotalsByCurrency = new Dictionary<int, decimal>();
			_expectedPortionOfCurrency = new Dictionary<int, decimal>();
			_expectedPaytopiaTotalsByCurrency = new Dictionary<int, decimal>();
			_expectedPaytopiaPortionOfCurrency = new Dictionary<int, decimal>();

			_users = GenerateUsers();
			_currencies = GenerateCurrencies();
			_tradePairs = GenerateTradePairs();
			_tradeHistories = GenerateTradeHistories();
			
			var mockUsers = new Mock<DbSet<User>>();
			mockUsers.As<IQueryable<User>>().Setup(m => m.Provider).Returns(new TestDbAsyncQueryProvider<User>(_users.Provider));
			mockUsers.As<IQueryable<User>>().Setup(m => m.Expression).Returns(_users.Expression);
			mockUsers.As<IQueryable<User>>().Setup(m => m.ElementType).Returns(_users.ElementType);
			mockUsers.As<IDbAsyncEnumerable<User>>().Setup(m => m.GetAsyncEnumerator()).Returns(new TestDbAsyncEnumerator<User>(_users.GetEnumerator()));

			var mockCurrencies = new Mock<DbSet<Currency>>();
			mockCurrencies.As<IQueryable<Currency>>().Setup(x => x.Provider).Returns(new TestDbAsyncQueryProvider<Currency>(_currencies.Provider));
			mockCurrencies.As<IQueryable<Currency>>().Setup(x => x.Expression).Returns(_currencies.Expression);
			mockCurrencies.As<IQueryable<Currency>>().Setup(x => x.ElementType).Returns(_currencies.ElementType);
			mockCurrencies.As<IDbAsyncEnumerable<Currency>>().Setup(x => x.GetAsyncEnumerator()).Returns(new TestDbAsyncEnumerator<Currency>(_currencies.GetEnumerator()));

			var mockPairs = new Mock<DbSet<TradePair>>();
			mockPairs.As<IQueryable<TradePair>>().Setup(x => x.Provider).Returns(new TestDbAsyncQueryProvider<TradePair>(_tradePairs.Provider));
			mockPairs.As<IQueryable<TradePair>>().Setup(x => x.Expression).Returns(_tradePairs.Expression);
			mockPairs.As<IQueryable<TradePair>>().Setup(x => x.ElementType).Returns(_tradePairs.ElementType);
			mockPairs.As<IDbAsyncEnumerable<TradePair>>().Setup(x => x.GetAsyncEnumerator()).Returns(new TestDbAsyncEnumerator<TradePair>(_tradePairs.GetEnumerator()));

			var mockHistories = new Mock<DbSet<TradeHistory>>();
			mockHistories.As<IQueryable<TradeHistory>>().Setup(x => x.Provider).Returns(new TestDbAsyncQueryProvider<TradeHistory>(_tradeHistories.Provider));
			mockHistories.As<IQueryable<TradeHistory>>().Setup(x => x.Expression).Returns(_tradeHistories.Expression);
			mockHistories.As<IQueryable<TradeHistory>>().Setup(x => x.ElementType).Returns(_tradeHistories.ElementType);
			mockHistories.As<IDbAsyncEnumerable<TradeHistory>>().Setup(x => x.GetAsyncEnumerator()).Returns(new TestDbAsyncEnumerator<TradeHistory>(_tradeHistories.GetEnumerator()));

			_exchangeContextFactory = new Mock_ExchangeDataContextFactory
			{
				_context = new Mock_ExchangeDataContext
				{
					Users = mockUsers.Object,
					Currency = mockCurrencies.Object,
					TradePair = mockPairs.Object,
					TradeHistory = mockHistories.Object
				}
			};

			GeneratePaytopiaPaymentsAndItems(out _paytopiaPayments, out _paytopiaPaymentItems);

			var mockPaymentItems = new Mock<DbSet<PaytopiaItem>>();
			mockPaymentItems.As<IQueryable<PaytopiaItem>>().Setup(x => x.Provider).Returns(new TestDbAsyncQueryProvider<PaytopiaItem>(_paytopiaPaymentItems.Provider));
			mockPaymentItems.As<IQueryable<PaytopiaItem>>().Setup(x => x.Expression).Returns(_paytopiaPaymentItems.Expression);
			mockPaymentItems.As<IQueryable<PaytopiaItem>>().Setup(x => x.ElementType).Returns(_paytopiaPaymentItems.ElementType);
			mockPaymentItems.As<IDbAsyncEnumerable<PaytopiaItem>>().Setup(x => x.GetAsyncEnumerator()).Returns(new TestDbAsyncEnumerator<PaytopiaItem>(_paytopiaPaymentItems.GetEnumerator()));

			var mockPayments = new Mock<DbSet<PaytopiaPayment>>();
			mockPayments.As<IQueryable<PaytopiaPayment>>().Setup(x => x.Provider).Returns(new TestDbAsyncQueryProvider<PaytopiaPayment>(_paytopiaPayments.Provider));
			mockPayments.As<IQueryable<PaytopiaPayment>>().Setup(x => x.Expression).Returns(_paytopiaPayments.Expression);
			mockPayments.As<IQueryable<PaytopiaPayment>>().Setup(x => x.ElementType).Returns(_paytopiaPayments.ElementType);
			mockPayments.As<IDbAsyncEnumerable<PaytopiaPayment>>().Setup(x => x.GetAsyncEnumerator()).Returns(new TestDbAsyncEnumerator<PaytopiaPayment>(_paytopiaPayments.GetEnumerator()));

			_dataContextFactory = new Mock_DataContextFactory
			{
				_context = new Mock_DataContext
				{
					PaytopiaItems = mockPaymentItems.Object,
					PaytopiaPayments = mockPayments.Object
				}
			};

			_processor = new CEFSProcessor(new CancellationToken());
			_processor.ExchangeDataContextFactory = _exchangeContextFactory;
			_processor.DataContextFactory = _dataContextFactory;
		}

		[TestMethod]
		public async Task Test_DateRange()
		{
			DateTime timestamp;

			using (var context = _exchangeContextFactory.CreateReadOnlyContext())
			{
				var tradeHistory = await context.TradeHistory.FirstOrDefaultAsync();
				timestamp = tradeHistory.Timestamp;
			}

			Assert.AreEqual(_testMonth, timestamp);
			Assert.IsTrue((((DateTime.UtcNow.Year - _testMonth.Year) * 12) + DateTime.UtcNow.Month - _testMonth.Month) == 1);
		}

		[TestMethod]
		public void Test_DataGeneration()
		{
			Assert.AreEqual(_users.Count(), MAX_USER_COUNT);
			Assert.AreEqual(_currencies.Count(), 15);
			Assert.AreEqual(_tradePairs.Count(), 60);
			Assert.AreEqual(_tradeHistories.Count(), 10000);

			Assert.AreEqual(_paytopiaPaymentItems.Count(), 1000);
			Assert.AreEqual(_paytopiaPayments.Count(), 1000);
		}

		[TestMethod]
		public async Task Test_AccessTradeHistories()
		{
			using (var context = _exchangeContextFactory.CreateReadOnlyContext())
			{
				var histories = await context.TradeHistory.ToListAsync();
				Assert.AreEqual(histories.Count(), 10000);

				var start = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month - 1, 1);
				var end = start.AddMonths(1);

				Assert.AreNotEqual(start, end);

				var query = await (from h in context.TradeHistory
								   join tp in context.TradePair on h.TradePairId equals tp.Id
								   join c in context.Currency on tp.CurrencyId2 equals c.Id
								   where h.Timestamp < end
								   && h.Timestamp >= start
								   group h.Fee by c.Id into g
								   select new { CurrencyId = g.Key, TotalFees = g.Sum() * 2 }).ToListAsync();

				Assert.IsTrue(query.Count() > 0);
			}
		}

		[TestMethod]
		public async Task Test_CalculateFeeTotalsAndPortions()
		{
			var utcNow = DateTime.UtcNow;
			DateTime monthToProcess = new DateTime(utcNow.Year, utcNow.Month - 1, 1);
			_tradeHistoryFeeByCurrency = new Dictionary<int, decimal>();
			_paytopiaPaymentTotalByCurrency = new Dictionary<int, decimal>();
			_tradeHistoryFeePortionsByCurrency = new Dictionary<int, decimal>();
			_paytopiaPaymentPortionsByCurrency = new Dictionary<int, decimal>();

			await _processor.CalculateFeeTotalsAndPortions(monthToProcess, _tradeHistoryFeeByCurrency, _paytopiaPaymentTotalByCurrency, _tradeHistoryFeePortionsByCurrency, _paytopiaPaymentPortionsByCurrency);

			Assert.AreEqual(_tradeHistoryFeeByCurrency.Keys.Count, _expectedFeeTotalsByCurrency.Keys.Count);
			Assert.AreEqual(_paytopiaPaymentTotalByCurrency.Keys.Count, _expectedPaytopiaTotalsByCurrency.Keys.Count);
			Assert.AreEqual(_tradeHistoryFeePortionsByCurrency.Keys.Count, _expectedPortionOfCurrency.Keys.Count);
			Assert.AreEqual(_paytopiaPaymentPortionsByCurrency.Keys.Count, _expectedPaytopiaPortionOfCurrency.Keys.Count);

			foreach (var kvp in _tradeHistoryFeeByCurrency)
			{
				Assert.AreEqual(_expectedFeeTotalsByCurrency[kvp.Key], kvp.Value);
			}

			foreach (var kvp in _paytopiaPaymentTotalByCurrency)
			{
				Assert.AreEqual(_expectedPaytopiaTotalsByCurrency[kvp.Key], kvp.Value);
			}

			foreach (var kvp in _tradeHistoryFeePortionsByCurrency)
			{
				Assert.AreEqual(_expectedPortionOfCurrency[kvp.Key], kvp.Value);
			}

			foreach (var kvp in _paytopiaPaymentPortionsByCurrency)
			{
				Assert.AreEqual(_expectedPaytopiaPortionOfCurrency[kvp.Key], kvp.Value);
			}
		}

		[TestMethod]
		public void Test_TruncateDecimal()
		{
			decimal initial = (decimal)9.876543210123456789;
			decimal expected = (decimal)9.87654321;

			decimal result = _processor.TruncateDecimal(initial, 8);

			Assert.AreEqual(result, expected);

			initial = (decimal)1.9898989898989898;
			expected = (decimal)1.98989898;

			result = _processor.TruncateDecimal(initial, 8);

			Assert.AreEqual(result, expected);

			initial = (decimal)7.8903465198024656;
			expected = (decimal)7.890346519802;

			result = _processor.TruncateDecimal(initial, 12);

			Assert.AreEqual(result, expected);

			initial = (decimal)8.2356971021;
			expected = (decimal)8.2356;

			result = _processor.TruncateDecimal(initial, 4);

			Assert.AreEqual(result, expected);
		}

		#region Test Setup Code

		private IQueryable<Currency> GenerateCurrencies()
		{
			return new List<Currency>
			{
				new Currency { Id = 1, Name = "Bitcoin", Symbol = "BTC", },
				new Currency { Id = 3, Name = "LiteCoin", Symbol = "LTC" },
				new Currency { Id = 4, Name = "Dogecoin", Symbol = "DOGE" },
				new Currency { Id = 5, Name = "NZDT", Symbol = "NZDT" },
				new Currency { Id = 6, Name = "USDT", Symbol = "USDT" },
				new Currency { Id = 990, Name = "Testcoin0", Symbol = "TST0" },
				new Currency { Id = 991, Name = "Testcoin1", Symbol = "TST1" },
				new Currency { Id = 992, Name = "Testcoin2", Symbol = "TST2" },
				new Currency { Id = 993, Name = "Testcoin3", Symbol = "TST3" },
				new Currency { Id = 994, Name = "Testcoin4", Symbol = "TST4" },
				new Currency { Id = 995, Name = "Testcoin5", Symbol = "TST5" },
				new Currency { Id = 996, Name = "Testcoin6", Symbol = "TST6" },
				new Currency { Id = 997, Name = "Testcoin7", Symbol = "TST7" },
				new Currency { Id = 998, Name = "Testcoin8", Symbol = "TST8" },
				new Currency { Id = 999, Name = "Testcoin9", Symbol = "TST9" }
			}.AsQueryable();
		}

		private IQueryable<TradePair> GenerateTradePairs()
		{
			return new List<TradePair>
			{
				new TradePair{ Id = 1, CurrencyId1 = 3, CurrencyId2 = 1 },
				new TradePair{ Id = 2, CurrencyId1 = 4, CurrencyId2 = 1 },
				new TradePair{ Id = 3, CurrencyId1 = 5, CurrencyId2 = 1 },
				new TradePair{ Id = 4, CurrencyId1 = 6, CurrencyId2 = 1 },
				new TradePair{ Id = 5, CurrencyId1 = 990, CurrencyId2 = 1 },
				new TradePair{ Id = 6, CurrencyId1 = 991, CurrencyId2 = 1 },
				new TradePair{ Id = 7, CurrencyId1 = 992, CurrencyId2 = 1 },
				new TradePair{ Id = 8, CurrencyId1 = 993, CurrencyId2 = 1 },
				new TradePair{ Id = 9, CurrencyId1 = 994, CurrencyId2 = 1 },
				new TradePair{ Id = 10, CurrencyId1 = 995, CurrencyId2 = 1 },
				new TradePair{ Id = 11, CurrencyId1 = 996, CurrencyId2 = 1 },
				new TradePair{ Id = 12, CurrencyId1 = 997, CurrencyId2 = 1 },
				new TradePair{ Id = 13, CurrencyId1 = 998, CurrencyId2 = 1 },
				new TradePair{ Id = 14, CurrencyId1 = 999, CurrencyId2 = 1 },

				new TradePair{ Id = 15, CurrencyId1 = 4, CurrencyId2 = 3 },
				new TradePair{ Id = 16, CurrencyId1 = 5, CurrencyId2 = 3 },
				new TradePair{ Id = 17, CurrencyId1 = 6, CurrencyId2 = 3 },
				new TradePair{ Id = 18, CurrencyId1 = 990, CurrencyId2 = 3 },
				new TradePair{ Id = 19, CurrencyId1 = 991, CurrencyId2 = 3 },
				new TradePair{ Id = 20, CurrencyId1 = 992, CurrencyId2 = 3 },
				new TradePair{ Id = 21, CurrencyId1 = 993, CurrencyId2 = 3 },
				new TradePair{ Id = 22, CurrencyId1 = 994, CurrencyId2 = 3 },
				new TradePair{ Id = 23, CurrencyId1 = 995, CurrencyId2 = 3 },
				new TradePair{ Id = 24, CurrencyId1 = 996, CurrencyId2 = 3 },
				new TradePair{ Id = 25, CurrencyId1 = 997, CurrencyId2 = 3 },
				new TradePair{ Id = 26, CurrencyId1 = 998, CurrencyId2 = 3 },
				new TradePair{ Id = 27, CurrencyId1 = 999, CurrencyId2 = 3 },


				new TradePair{ Id = 28, CurrencyId1 = 999, CurrencyId2 = 4 },
				new TradePair{ Id = 29, CurrencyId1 = 990, CurrencyId2 = 4 },
				new TradePair{ Id = 30, CurrencyId1 = 991, CurrencyId2 = 4 },
				new TradePair{ Id = 31, CurrencyId1 = 992, CurrencyId2 = 4 },
				new TradePair{ Id = 32, CurrencyId1 = 993, CurrencyId2 = 4 },
				new TradePair{ Id = 33, CurrencyId1 = 994, CurrencyId2 = 4 },
				new TradePair{ Id = 34, CurrencyId1 = 995, CurrencyId2 = 4 },
				new TradePair{ Id = 35, CurrencyId1 = 996, CurrencyId2 = 4 },
				new TradePair{ Id = 36, CurrencyId1 = 997, CurrencyId2 = 4 },
				new TradePair{ Id = 37, CurrencyId1 = 998, CurrencyId2 = 4 },
				new TradePair{ Id = 38, CurrencyId1 = 999, CurrencyId2 = 4 },

				new TradePair{ Id = 39, CurrencyId1 = 999, CurrencyId2 = 5 },
				new TradePair{ Id = 40, CurrencyId1 = 990, CurrencyId2 = 5 },
				new TradePair{ Id = 41, CurrencyId1 = 991, CurrencyId2 = 5 },
				new TradePair{ Id = 42, CurrencyId1 = 992, CurrencyId2 = 5 },
				new TradePair{ Id = 43, CurrencyId1 = 993, CurrencyId2 = 5 },
				new TradePair{ Id = 44, CurrencyId1 = 994, CurrencyId2 = 5 },
				new TradePair{ Id = 45, CurrencyId1 = 995, CurrencyId2 = 5 },
				new TradePair{ Id = 46, CurrencyId1 = 996, CurrencyId2 = 5 },
				new TradePair{ Id = 47, CurrencyId1 = 997, CurrencyId2 = 5 },
				new TradePair{ Id = 48, CurrencyId1 = 998, CurrencyId2 = 5 },
				new TradePair{ Id = 49, CurrencyId1 = 999, CurrencyId2 = 5 },

				new TradePair{ Id = 50, CurrencyId1 = 999, CurrencyId2 = 6 },
				new TradePair{ Id = 51, CurrencyId1 = 990, CurrencyId2 = 6 },
				new TradePair{ Id = 52, CurrencyId1 = 991, CurrencyId2 = 6 },
				new TradePair{ Id = 53, CurrencyId1 = 992, CurrencyId2 = 6 },
				new TradePair{ Id = 54, CurrencyId1 = 993, CurrencyId2 = 6 },
				new TradePair{ Id = 55, CurrencyId1 = 994, CurrencyId2 = 6 },
				new TradePair{ Id = 56, CurrencyId1 = 995, CurrencyId2 = 6 },
				new TradePair{ Id = 57, CurrencyId1 = 996, CurrencyId2 = 6 },
				new TradePair{ Id = 58, CurrencyId1 = 997, CurrencyId2 = 6 },
				new TradePair{ Id = 59, CurrencyId1 = 998, CurrencyId2 = 6 },
				new TradePair{ Id = 60, CurrencyId1 = 999, CurrencyId2 = 6 },
			}.AsQueryable();
		}

		private IQueryable<User> GenerateUsers()
		{
			int count = 0;
			var users = new List<User>();

			while (count < MAX_USER_COUNT)
			{
				users.Add(new User { Id = Guid.NewGuid(), UserName = $"TestUser{count}" });
				count++;
			}

			return users.AsQueryable();
		}

		private IQueryable<TradeHistory> GenerateTradeHistories()
		{
			int count = 0;
			Random random = new Random();
			var histories = new List<TradeHistory>();

			var totalFeesByCurrency = new Dictionary<int, decimal>();

			while (count < 10000)
			{
				Guid uId = _users.ElementAt(random.Next(1, _users.Count())).Id;
				Guid toUId = _users.ElementAt(random.Next(1, _users.Count())).Id;

				while (toUId == uId)
					toUId = _users.ElementAt(random.Next(1, _users.Count())).Id;

				decimal amount = random.Next(1, 1000000);
				decimal rate = (decimal)0.00000005;
				decimal fee = (amount * rate) * (decimal)0.2;

				var currencyId = 0;
				var rand = random.Next(1, 1000);
				if (rand > 250)
					currencyId = random.Next(990, 999);
				else
					currencyId = random.Next(1, 7);

				var pairs = _tradePairs.Where(x => x.CurrencyId1 == currencyId).ToList();

				if (!pairs.Any())
				{
					while (!pairs.Any())
					{
						currencyId = random.Next(1, 1000) > 250 ? random.Next(990, 999) : random.Next(1, 7);
						pairs = _tradePairs.Where(x => x.CurrencyId1 == currencyId).ToList();
					}
				}

				var tradePair = pairs[random.Next(0, pairs.Count - 1)];

				TradeHistory th = new TradeHistory
				{
					UserId = uId,
					ToUserId = toUId,
					CurrencyId = currencyId,
					TradePairId = tradePair.Id,
					Amount = amount,
					Rate = rate,
					Fee = fee,
					Timestamp = _testMonth
				};

				histories.Add(th);

				if (totalFeesByCurrency.ContainsKey(tradePair.CurrencyId2))
				{
					decimal current = totalFeesByCurrency[tradePair.CurrencyId2];
					totalFeesByCurrency[tradePair.CurrencyId2] = (current + fee);
				}
				else
					totalFeesByCurrency[tradePair.CurrencyId2] = fee;

				count++;
			}

			foreach (var kvp in totalFeesByCurrency)
			{
				// multiply by two for both sides of the transaction
				var total = (kvp.Value * 2);
				_expectedFeeTotalsByCurrency[kvp.Key] = total;
				_expectedPortionOfCurrency[kvp.Key] = ((total * FOUR_POINT_FIVE_PERCENT) / TOTAL_PORTIONS);
			}

			return histories.AsQueryable();
		}

		private void GeneratePaytopiaPaymentsAndItems(out IQueryable<PaytopiaPayment> payments, out IQueryable<PaytopiaItem> paymentItems)
		{
			List<PaytopiaPayment> _payments = new List<PaytopiaPayment>();
			List<PaytopiaItem> _paymentItems = new List<PaytopiaItem>();
			Random random = new Random();

			int count = 1;
			while (count <= 1000)
			{
				string uId = _users.ElementAt(random.Next(1, _users.Count())).Id.ToString();

				DateTime now = DateTime.UtcNow;

				_payments.Add(new PaytopiaPayment
				{
					Id = count,
					PaytopiaItemId = count,
					UserId = uId,
					Status = Cryptopia.Enums.PaytopiaPaymentStatus.Complete,
					Begins = now,
					Ends = now,
					Timestamp = _testMonth
				});

				decimal amount = random.Next(1, 100000);
				int typeid = random.Next(1, 12);

				var paymentItem = new PaytopiaItem
				{
					Id = count,
					Name = $"Paytopia Item {count} Generated",
					Description = "Generated for use in unit testing",
					Price = amount,
					CurrencyId = 2,
					Type = (Cryptopia.Enums.PaytopiaItemType)typeid,
					Period = Cryptopia.Enums.PaytopiaItemPeriod.Fixed,
					Timestamp = _testMonth
				};

				_paymentItems.Add(paymentItem);

				if (_expectedPaytopiaTotalsByCurrency.ContainsKey(paymentItem.CurrencyId))
				{
					decimal current = _expectedPaytopiaTotalsByCurrency[paymentItem.CurrencyId];
					_expectedPaytopiaTotalsByCurrency[paymentItem.CurrencyId] = (current + paymentItem.Price);
				}
				else
					_expectedPaytopiaTotalsByCurrency[paymentItem.CurrencyId] = paymentItem.Price;

				count++;
			}

			foreach (var kvp in _expectedPaytopiaTotalsByCurrency)
			{
				_expectedPaytopiaPortionOfCurrency[kvp.Key] = ((kvp.Value * FOUR_POINT_FIVE_PERCENT) / TOTAL_PORTIONS);
			}

			payments = _payments.AsQueryable();
			paymentItems = _paymentItems.AsQueryable();
		}

		#endregion
	}
}
