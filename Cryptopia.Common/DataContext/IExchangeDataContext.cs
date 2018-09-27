using Cryptopia.Entity;
using System;
using System.Data.Entity;
using System.Threading.Tasks;

namespace Cryptopia.Common.DataContext
{
	public interface IExchangeDataContext : IDisposable
	{
		Database Database { get; }
		int SaveChanges();
		Task<int> SaveChangesAsync();
		Task<bool> AuditUserBalance(Guid userId, int currency);

		DbSet<Entity.User> Users { get; set; }
		DbSet<Entity.TransferHistory> Transfer { get; set; }
		DbSet<Entity.Withdraw> Withdraw { get; set; }
		DbSet<Entity.Deposit> Deposit { get; set; }
		DbSet<Entity.TradePair> TradePair { get; set; }
		DbSet<Entity.Trade> Trade { get; set; }
		DbSet<Entity.TradeHistory> TradeHistory { get; set; }
		DbSet<Entity.AddressBook> AddressBook { get; set; }
		DbSet<Entity.Balance> Balance { get; set; }
		DbSet<Entity.Address> Address { get; set; }
		DbSet<Entity.Currency> Currency { get; set; }
		DbSet<Entity.CurrencyInfo> CurrencyInfo { get; set; }
		DbSet<Entity.CurrencySettings> CurrencySettings { get; set; }

		DbSet<Entity.MarketCategory> MarketCategory { get; set; }
		DbSet<Entity.MarketFeedback> MarketFeedback { get; set; }
		DbSet<Entity.MarketItem> MarketItem { get; set; }
		DbSet<Entity.MarketItemBid> MarketItemBid { get; set; }
		DbSet<Entity.MarketItemImage> MarketItemImage { get; set; }
		DbSet<Entity.MarketItemQuestion> MarketItemQuestion { get; set; }

		DbSet<Entity.Location> Location { get; set; }

		DbSet<LottoItem> LottoItem { get; set; }
		DbSet<LottoTicket> LottoTicket { get; set; }
		DbSet<LottoHistory> LottoHistory { get; set; }

		DbSet<Reward> Reward { get; set; }

		DbSet<IntegrationExchange> IntegrationExchange { get; set; }
		DbSet<IntegrationMarketData> IntegrationMarketData { get; set; }

		DbSet<TermDeposit> TermDeposit { get; set; }
		DbSet<TermDepositItem> TermDepositItem { get; set; }
		DbSet<TermDepositPayment> TermDepositPayment { get; set; }

		DbSet<ReferralInfo> ReferralInfo { get; set; }
		DbSet<ExternalTransaction> ExternalTransaction { get; set; }
		DbSet<NzdtTransaction> NzdtTransaction { get; set; }

	}
}