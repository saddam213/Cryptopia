using System;
using System.Linq;
using System.Threading.Tasks;
using Cryptopia.Common.DataContext;
using Cryptopia.Infrastructure.Common.DataContext;
using Cryptopia.Infrastructure.Common.DataTables;
using Cryptopia.Common.Withdraw;

namespace Cryptopia.Core.Withdraw
{
	public class WithdrawReader : IWithdrawReader
	{
		public IExchangeDataContextFactory ExchangeDataContextFactory { get; set; }

		public async Task<WithdrawModel> GetWithdrawal(string userId, int withdrawId)
		{
			var currentUser = new Guid(userId);
			using (var context = ExchangeDataContextFactory.CreateReadOnlyContext())
			{
				return await context.Withdraw
					.AsNoTracking()
					.Where(x => x.UserId == currentUser && x.Id == withdrawId)
					.Select(withdraw => new WithdrawModel
					{
						Id = withdraw.Id,
						Currency = withdraw.Currency.Symbol,
						AddressType = withdraw.Currency.Settings.AddressType,
						Amount = withdraw.Amount,
						Fee = withdraw.Fee,
						Address = withdraw.Address,
						DisableWithdrawEmailConfirmation = withdraw.User.DisableWithdrawEmailConfirmation,
						Email = withdraw.User.Email
					}).FirstOrDefaultNoLockAsync().ConfigureAwait(false);
			}
		}

		public async Task<DataTablesResponse> GetWithdrawals(string userId, DataTablesModel model)
		{
			using (var context = ExchangeDataContextFactory.CreateReadOnlyContext())
			{
				var query = context.Withdraw
					.AsNoTracking()
					.Select(withdraw => new
					{
						Id = withdraw.Id,
						User = withdraw.User.UserName,
						Currency = withdraw.Currency.Symbol,
						Amount = withdraw.Amount - withdraw.Fee,
						Status = withdraw.Status,
						TxId = withdraw.Txid,
						Address = withdraw.Address,
						Conf = withdraw.Confirmations,
						Timestamp = withdraw.TimeStamp
					});

				return await query.GetDataTableResultNoLockAsync(model).ConfigureAwait(false);
			}
		}
	}
}