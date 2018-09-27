using System;
using System.Data.Entity;
using System.Threading.Tasks;

using Cryptopia.Common.DataContext;
using Cryptopia.Entity;

namespace IntegrationServiceTests.DataContext
{
	public class Mock_ExchangeDataContext : IExchangeDataContext
	{
		public Database Database => throw new NotImplementedException();

		public DbSet<User> Users { get; set; }
		public DbSet<TransferHistory> Transfer { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public DbSet<Withdraw> Withdraw { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public DbSet<Deposit> Deposit { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public DbSet<TradePair> TradePair { get; set; }
		public DbSet<Trade> Trade { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public DbSet<TradeHistory> TradeHistory { get; set; }
		public DbSet<AddressBook> AddressBook { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public DbSet<Balance> Balance { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public DbSet<Address> Address { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public DbSet<Currency> Currency { get; set; }
		public DbSet<CurrencyInfo> CurrencyInfo { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public DbSet<CurrencySettings> CurrencySettings { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public DbSet<MarketCategory> MarketCategory { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public DbSet<MarketFeedback> MarketFeedback { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public DbSet<MarketItem> MarketItem { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public DbSet<MarketItemBid> MarketItemBid { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public DbSet<MarketItemImage> MarketItemImage { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public DbSet<MarketItemQuestion> MarketItemQuestion { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public DbSet<Location> Location { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public DbSet<LottoItem> LottoItem { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public DbSet<LottoTicket> LottoTicket { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public DbSet<LottoHistory> LottoHistory { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public DbSet<Reward> Reward { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public DbSet<IntegrationExchange> IntegrationExchange { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public DbSet<IntegrationMarketData> IntegrationMarketData { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public DbSet<TermDeposit> TermDeposit { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public DbSet<TermDepositItem> TermDepositItem { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public DbSet<TermDepositPayment> TermDepositPayment { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public DbSet<ReferralInfo> ReferralInfo { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public DbSet<ExternalTransaction> ExternalTransaction { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

		public DbSet<NzdtTransaction> NzdtTransaction { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

		public Task<bool> AuditUserBalance(Guid userId, int currency)
		{
			return Task.FromResult(true);
		}

		public void Dispose()
		{
			
		}

		public int SaveChanges()
		{
			return 0;
		}

		public Task<int> SaveChangesAsync()
		{
			return Task.FromResult(0);
		}
	}
}
