using Cryptopia.Common.DataContext;
using Cryptopia.Entity;
using System;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Cryptopia.Data.DataContext
{
	public class ExchangeDataContext : DbContext, IExchangeDataContext
	{
		public ExchangeDataContext()
			: base("DefaultExchangeConnection")
		{
			Database.Log = (e) => Debug.WriteLine(e);
		}

		public DbSet<Entity.User> Users { get; set; }
		public DbSet<Entity.Currency> Currency { get; set; }
		public DbSet<CurrencyInfo> CurrencyInfo { get; set; }
		public DbSet<CurrencySettings> CurrencySettings { get; set; }
		public DbSet<Entity.TransferHistory> Transfer { get; set; }
		public DbSet<Entity.Withdraw> Withdraw { get; set; }
		public DbSet<Entity.Deposit> Deposit { get; set; }
		public DbSet<Entity.TradePair> TradePair { get; set; }
		public DbSet<Entity.Trade> Trade { get; set; }
		public DbSet<Entity.TradeHistory> TradeHistory { get; set; }
		public DbSet<Entity.AddressBook> AddressBook { get; set; }
		public DbSet<Entity.Balance> Balance { get; set; }
		public DbSet<Entity.Address> Address { get; set; }

		public DbSet<Entity.MarketCategory> MarketCategory { get; set; }
		public DbSet<Entity.MarketFeedback> MarketFeedback { get; set; }
		public DbSet<Entity.MarketItem> MarketItem { get; set; }
		public DbSet<Entity.MarketItemBid> MarketItemBid { get; set; }
		public DbSet<Entity.MarketItemImage> MarketItemImage { get; set; }
		public DbSet<Entity.MarketItemQuestion> MarketItemQuestion { get; set; }

		public DbSet<Entity.Location> Location { get; set; }

		public DbSet<LottoItem> LottoItem { get; set; }
		public DbSet<LottoTicket> LottoTicket { get; set; }
		public DbSet<LottoHistory> LottoHistory { get; set; }

		public DbSet<Reward> Reward { get; set; }

		public DbSet<IntegrationExchange> IntegrationExchange { get; set; }
		public DbSet<IntegrationMarketData> IntegrationMarketData { get; set; }

		public DbSet<TermDeposit> TermDeposit { get; set; }
		public DbSet<TermDepositItem> TermDepositItem { get; set; }
		public DbSet<TermDepositPayment> TermDepositPayment { get; set; }

		public DbSet<ReferralInfo> ReferralInfo { get; set; }
		public DbSet<ExternalTransaction> ExternalTransaction { get; set; }
		public DbSet<NzdtTransaction> NzdtTransaction { get; set; }

		public async Task<bool> AuditUserBalance(Guid userId, int currency)
		{
			var userIdParameter = new SqlParameter("@UserId", userId);
			var currencyParameter = new SqlParameter("@CurrencyId", currency);
			if (await Database.ExecuteSqlCommandAsync("EXEC AuditUserBalance @UserId, @CurrencyId", userIdParameter, currencyParameter) > 0)
			{
				return true;
			}
			return false;
		}
		
		protected override void OnModelCreating(DbModelBuilder modelBuilder)
		{
			modelBuilder.Conventions.Remove<DecimalPropertyConvention>();
			modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
			modelBuilder.Conventions.Add(new DecimalPropertyConvention(38, 8));
		//	modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
			modelBuilder.Properties<decimal>().Configure(config => config.HasPrecision(38, 8));

			modelBuilder.Entity<Entity.User>().ToTable("User");

			modelBuilder.Entity<TransferHistory>().HasRequired(p => p.User);
			modelBuilder.Entity<TransferHistory>().HasRequired(p => p.ToUser);
			modelBuilder.Entity<TransferHistory>().HasRequired(p => p.Currency);

			modelBuilder.Entity<Deposit>().HasRequired(p => p.User);
			modelBuilder.Entity<Deposit>().HasRequired(p => p.Currency);

			modelBuilder.Entity<Withdraw>().HasRequired(p => p.User);
			modelBuilder.Entity<Withdraw>().HasRequired(p => p.Currency);

			modelBuilder.Entity<Trade>().HasRequired(p => p.User);
			modelBuilder.Entity<Trade>().HasRequired(p => p.TradePair);

			modelBuilder.Entity<TradeHistory>().HasRequired(p => p.User);
			modelBuilder.Entity<TradeHistory>().HasRequired(p => p.ToUser);
			modelBuilder.Entity<TradeHistory>().HasRequired(p => p.TradePair);

			modelBuilder.Entity<TradePair>().HasRequired(p => p.Currency1);
			modelBuilder.Entity<TradePair>().HasRequired(p => p.Currency2);

			modelBuilder.Entity<AddressBook>().HasRequired(p => p.User);
			modelBuilder.Entity<AddressBook>().HasRequired(p => p.Currency);

			modelBuilder.Entity<Balance>().HasRequired(p => p.User);
			modelBuilder.Entity<Balance>().HasRequired(p => p.Currency);

			modelBuilder.Entity<Address>().HasRequired(p => p.User);
			modelBuilder.Entity<Address>().HasRequired(p => p.Currency);

			modelBuilder.Entity<Location>().HasOptional(p => p.Parent);

			modelBuilder.Entity<MarketCategory>().HasOptional(p => p.Parent);

			modelBuilder.Entity<MarketFeedback>().HasRequired(p => p.SenderUser);
			modelBuilder.Entity<MarketFeedback>().HasRequired(p => p.ReceiverUser);
			modelBuilder.Entity<MarketFeedback>().HasRequired(p => p.MarketItem)
				.WithMany(p => p.Feedback)
				.HasForeignKey(p => p.MarketItemId);
			;

			modelBuilder.Entity<MarketItem>().HasRequired(p => p.User);
			modelBuilder.Entity<MarketItem>().HasRequired(p => p.Category);
			modelBuilder.Entity<MarketItem>().HasRequired(p => p.Currency);
			modelBuilder.Entity<MarketItem>().HasRequired(p => p.Location);

			modelBuilder.Entity<MarketItemBid>().HasRequired(p => p.User);
			modelBuilder.Entity<MarketItemBid>()
				.HasRequired(p => p.MarketItem)
				.WithMany(p => p.Bids)
				.HasForeignKey(p => p.MarketItemId);

			modelBuilder.Entity<MarketItemImage>()
				.HasRequired(p => p.MarketItem)
				.WithMany(p => p.Images)
				.HasForeignKey(p => p.MarketItemId);

			modelBuilder.Entity<MarketItemQuestion>().HasRequired(p => p.User);
			modelBuilder.Entity<MarketItemQuestion>()
				.HasRequired(p => p.MarketItem)
				.WithMany(p => p.Questions)
				.HasForeignKey(p => p.MarketItemId);

			modelBuilder.Entity<Currency>().HasRequired(p => p.Info).WithRequiredPrincipal(x => x.Currency);
			modelBuilder.Entity<Currency>().HasRequired(p => p.Settings).WithRequiredPrincipal(x => x.Currency);
			//modelBuilder.Entity<CurrencyRating>().HasRequired(p => p.Currency).WithRequiredDependent();

			modelBuilder.Entity<LottoItem>().HasRequired(p => p.Currency);
			modelBuilder.Entity<LottoTicket>().HasRequired(p => p.User);
			modelBuilder.Entity<LottoTicket>().HasRequired(p => p.LottoItem).WithMany(p => p.Tickets);
			modelBuilder.Entity<LottoHistory>().HasRequired(p => p.User);
			modelBuilder.Entity<LottoHistory>().HasRequired(p => p.LottoItem);

			modelBuilder.Entity<Reward>().HasRequired(p => p.Currency);
			modelBuilder.Entity<Reward>().HasRequired(p => p.User);

			modelBuilder.Entity<IntegrationMarketData>().HasRequired(p => p.TradePair);
			modelBuilder.Entity<IntegrationMarketData>().HasRequired(p => p.IntegrationExchange);

			modelBuilder.Entity<TermDepositItem>().HasMany(p => p.TermDeposits);

			modelBuilder.Entity<TermDeposit>().HasRequired(p => p.User);
			modelBuilder.Entity<TermDeposit>().HasRequired(p => p.Withdraw);
			modelBuilder.Entity<TermDeposit>().HasRequired(p => p.TermDepositItem);

			modelBuilder.Entity<TermDepositPayment>().HasRequired(p => p.TermDeposit).WithMany(x => x.TermDepositPayments).WillCascadeOnDelete(false);

			modelBuilder.Entity<ReferralInfo>().HasRequired(p => p.User);

			modelBuilder.Entity<NzdtTransaction>().HasOptional(t => t.Deposit);
			modelBuilder.Entity<NzdtTransaction>().HasOptional(t => t.User);
		}
	}
}