using Cryptopia.Entity;
using Cryptopia.Entity.Auditing;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Cryptopia.Data.DataContext
{
	public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
	{
		public ApplicationDbContext()
			: base("DefaultConnection", false)
		{
			Database.Log = (e) => Debug.WriteLine(e);
		}

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
		public DbSet<TwoFactorCode> TwoFactorCode { get; set; }
        public DbSet<ActivityLog> AuditLog { get; set; }


        protected override void OnModelCreating(DbModelBuilder modelBuilder)
		{
			modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
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

			modelBuilder.Entity<UserKarma>().HasRequired(k => k.User);
			modelBuilder.Entity<UserKarma>().HasRequired(k => k.Sender);

			base.OnModelCreating(modelBuilder);
		}

		public static ApplicationDbContext Create()
		{
			return new ApplicationDbContext();
		}

		public ApplicationDbContext CreateContextWithNoLock()
		{
			var context = new ApplicationDbContext();
			context.Database.ExecuteSqlCommand("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;");
			return context;
		}

        public override async Task<int> SaveChangesAsync()
        {
            Guid transactionId = Guid.NewGuid();
            var changed = ChangeTracker.Entries()
                          .Where(e => e.State == EntityState.Modified);

            foreach (DbEntityEntry entry in changed)
            {
                var entityType = ObjectContext.GetObjectType(entry.Entity.GetType());
                if (!typeof(IAuditable).IsAssignableFrom(entityType))
                    break;

                if (entityType is IAuditable)
                {
                    var fullProperties = entityType.GetProperties();
                    string userId = entry.OriginalValues["Id"].ToString();
                    Guid _;
                    if (!Guid.TryParse(userId, out _))
                        userId = entry.OriginalValues["UserId"].ToString();

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
                                    AdminUserId = default(Guid?)
                                });
                            }
                        }
                    }
                }
            }

            return await base.SaveChangesAsync();
        }
    }
}