namespace Cryptopia.Data.DataContext
{
    using System.Data.Entity;
    using System.Data.Entity.ModelConfiguration.Conventions;
    using System.Diagnostics;
    using System.Threading.Tasks;
    using Cryptopia.Entity;
    using Cryptopia.Entity.Support;
    using Cryptopia.Infrastructure.Common.DataContext;
    using Microsoft.AspNet.Identity.EntityFramework;
    using System.Linq;
    using Cryptopia.Entity.Auditing;
    using System.Reflection;
    using System.Data.Entity.Core.Objects;
    using System;
    using System.Data.Entity.Infrastructure;

    public class DataContext : DbContext, IDataContext
	{
		public DataContext()
			: base("DefaultConnection")
		{
			Database.Log = e => Debug.WriteLine(e);
		}

		public DbSet<ApplicationUser> Users { get; set; }
		public DbSet<IdentityRole> Roles { get; set; }
		public DbSet<UserRole> UserRoles { get; set; }
		public DbSet<ChatMessage> ChatMessages { get; set; }
		public DbSet<UserSettings> UserSettings { get; set; }
		public DbSet<UserTwoFactor> UserTwoFactor { get; set; }
		public DbSet<EmailTemplate> EmailTemplates { get; set; }
		public DbSet<UserNotification> Notifications { get; set; }
		public DbSet<UserMessage> Messages { get; set; }
		public DbSet<UserProfile> UserProfiles { get; set; }
		public DbSet<UserLogon> UserLogons { get; set; }
		public DbSet<Settings> Settings { get; set; }
		public DbSet<Forum> Forums { get; set; }
		public DbSet<ForumCategory> ForumCategories { get; set; }
		public DbSet<ForumThread> ForumThreads { get; set; }
		public DbSet<ForumPost> ForumPosts { get; set; }
		public DbSet<ForumReport> ForumReports { get; set; }
		public DbSet<UserKarma> UserKarma { get; set; }
		public DbSet<UserKarmaHistory> UserKarmaHistory { get; set; }

		public DbSet<SiteExpense> SiteExpenses { get; set; }

		public DbSet<PaytopiaItem> PaytopiaItems { get; set; }
		public DbSet<PaytopiaPayment> PaytopiaPayments { get; set; }
		public DbSet<NewsItem> NewsItem { get; set; }
		public DbSet<ApprovalQueue> ApprovalQueue { get; set; }
		public DbSet<UserVerification> UserVerification { get; set; }
		public DbSet<UserVerificationReject> UserVerificationReject { get; set; }

		public DbSet<TwoFactorCode> TwoFactorCode { get; set; }
		//public DbSet<EmailMessage> EmailMessages { get; set; }
		public DbSet<Entity.Support.SupportTicket> SupportTicket { get; set; }
		public DbSet<Entity.Support.SupportTicketMessage> SupportTicketMessage { get; set; }
		public DbSet<SupportTicketQueue> SupportTicketQueue { get; set; }
		public DbSet<AuthenticatedFeature> AuthenticatedFeature { get; set; }
		public DbSet<SupportTag> SupportTag { get; set; }

        public DbSet<ActivityLog> AuditLog { get; set; }
        public DbSet<AdminActivityLog> AdminActivityLog { get; set; }

        public void LogActivity(string adminUserId, string activity)
        {
            AdminActivityLog.Add(new Entity.AdminActivityLog
            {
                DateCreated = DateTime.UtcNow,
                AdminUserId = new Guid(adminUserId),
                ActivityDescription = activity
            });
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
		{
			modelBuilder.Conventions.Remove<DecimalPropertyConvention>();
			modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
			modelBuilder.Conventions.Add(new DecimalPropertyConvention(38, 8));

			modelBuilder.Entity<ApplicationUser>().ToTable("AspNetUsers");
			modelBuilder.Entity<IdentityRole>().ToTable("AspNetRoles");
			modelBuilder.Entity<UserRole>()
				.ToTable("AspNetUserRoles")
				.HasKey(r => new {r.RoleId, r.UserId})
				.HasRequired(p => p.User);
			modelBuilder.Entity<UserRole>().ToTable("AspNetUserRoles").HasRequired(p => p.Role);
			modelBuilder.Entity<IdentityUserLogin>().HasKey(l => l.UserId);
			modelBuilder.Entity<IdentityRole>().HasKey(r => r.Id);
			modelBuilder.Entity<IdentityUserRole>().HasKey(r => new {r.RoleId, r.UserId});

			modelBuilder.Entity<UserKarma>().HasRequired(p => p.User);
			modelBuilder.Entity<UserKarma>().HasRequired(p => p.Sender);
			modelBuilder.Entity<UserKarmaHistory>().HasRequired(p => p.User);

			modelBuilder.Entity<ChatMessage>()
				.HasRequired(p => p.User)
				.WithMany(b => b.ChatMessages)
				.HasForeignKey(p => p.UserId);
			modelBuilder.Entity<UserMessage>().HasRequired(p => p.User).WithMany(b => b.Messages).HasForeignKey(p => p.UserId);
			modelBuilder.Entity<UserNotification>()
				.HasRequired(p => p.User)
				.WithMany(b => b.Notifications)
				.HasForeignKey(p => p.UserId);
			modelBuilder.Entity<UserLogon>().HasRequired(p => p.User).WithMany(b => b.Logons).HasForeignKey(p => p.UserId);
			modelBuilder.Entity<UserTwoFactor>().HasRequired(p => p.User).WithMany(b => b.TwoFactor).HasForeignKey(p => p.UserId);
			modelBuilder.Entity<ApplicationUser>().HasRequired(p => p.Settings).WithRequiredDependent();
			modelBuilder.Entity<ApplicationUser>().HasRequired(p => p.Profile).WithRequiredDependent();

			modelBuilder.Entity<ForumThread>()
				.HasRequired(p => p.User)
				.WithMany(b => b.ForumThreads)
				.HasForeignKey(p => p.UserId)
				.WillCascadeOnDelete(false);
			modelBuilder.Entity<ForumPost>()
				.HasRequired(p => p.User)
				.WithMany(b => b.ForumPosts)
				.HasForeignKey(p => p.UserId)
				.WillCascadeOnDelete(false);

			modelBuilder.Entity<PaytopiaPayment>().HasRequired(p => p.User);
			modelBuilder.Entity<PaytopiaPayment>().HasRequired(p => p.PaytopiaItem).WithMany(s => s.Payments);

			modelBuilder.Entity<NewsItem>().HasRequired(k => k.User);

			modelBuilder.Entity<ApprovalQueue>().HasRequired(k => k.RequestUser);
			modelBuilder.Entity<ApprovalQueue>().HasRequired(k => k.DataUser);
			modelBuilder.Entity<ApprovalQueue>().HasOptional(k => k.ApproveUser);

			modelBuilder.Entity<UserVerification>().HasRequired(p => p.User);
			modelBuilder.Entity<UserVerification>().HasOptional(p => p.ApprovedByUser);

			modelBuilder.Entity<SupportTicketMessage>().HasRequired(v => v.Ticket).WithMany(v => v.Messages);
			modelBuilder.Entity<SupportTicketMessage>().HasRequired(p => p.Sender);
			modelBuilder.Entity<SupportTicket>().HasRequired(p => p.User);

			modelBuilder.Entity<SupportTicket>().HasRequired(t => t.Queue).WithMany(q => q.Tickets);

			modelBuilder.Entity<SupportTicket>().HasMany<SupportTag>(t => t.Tags).WithMany(t => t.Tickets).Map(x =>
			{
				x.MapLeftKey("TicketRefId");
				x.MapRightKey("TagRefId");
				x.ToTable("SupportTicketTag");
			});

			modelBuilder.Entity<AuthenticatedFeature>().HasRequired(f => f.TwoFactorCode);
		}

        public async Task<int> SaveChangesWithAuditAsync(string adminUserId = null)
        {
            Guid transactionId = Guid.NewGuid();
            var changed = ChangeTracker.Entries()
                            .Where(e => e.State == EntityState.Modified);

            Guid? adminGuid = string.IsNullOrEmpty(adminUserId) ? default(Guid?) : new Guid(adminUserId);

            foreach (DbEntityEntry entry in changed)
            {
                Type entityType = ObjectContext.GetObjectType(entry.Entity.GetType());
                if (!typeof(IAuditable).IsAssignableFrom(entityType))
                    break;

                string userId = entry.OriginalValues["Id"].ToString();
                Guid _;
                if (!Guid.TryParse(userId, out _))
                    userId = entry.OriginalValues["UserId"].ToString();

                PropertyInfo[] fullProperties = entityType.GetProperties();
                var interfaceProperties = entityType.GetProperties().Where(p => p.IsDefined(typeof(AuditableAttribute), false));

                if (interfaceProperties.Any())
                {
                    foreach (var prop in interfaceProperties)
                    {
                        var originalValue = entry.OriginalValues[prop.Name];
                        var newValue = entry.CurrentValues[prop.Name];

                        var customAttributes = prop.GetCustomAttribute<AuditableAttribute>();

                        if (customAttributes.LogAlways || !Equals(originalValue, newValue))
                        {
                            string auditLog = customAttributes.Obfuscate
                                ? $"{prop.Name} was updated"
                                : $"{prop.Name} was updated from {originalValue} to {newValue}";

                            AuditLog.Add(new Entity.ActivityLog
                            {
                                DateCreated = DateTime.UtcNow,
                                TxId = transactionId,
                                Entity = entityType.Name,
                                Property = prop.Name,
                                OldValue = customAttributes.Obfuscate ? null : originalValue.ToString(),
                                NewValue = customAttributes.Obfuscate ? null : newValue.ToString(),
                                UserId = new Guid(userId),
                                AdminUserId = adminGuid
                            });
                        }
                    }
                }
            }

            return await base.SaveChangesAsync();
        }
    }
}