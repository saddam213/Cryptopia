using System;
using System.Data.Entity;
using System.Threading.Tasks;

using Cryptopia.Entity;
using Cryptopia.Entity.Support;
using Cryptopia.Infrastructure.Common.DataContext;

namespace IntegrationServiceTests.DataContext
{
	public class Mock_DataContext : IDataContext
	{
		public Database Database => throw new NotImplementedException();

		public DbSet<ApplicationUser> Users { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public DbSet<Microsoft.AspNet.Identity.EntityFramework.IdentityRole> Roles { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public DbSet<UserRole> UserRoles { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public DbSet<ChatMessage> ChatMessages { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public DbSet<UserSettings> UserSettings { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public DbSet<UserTwoFactor> UserTwoFactor { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public DbSet<EmailTemplate> EmailTemplates { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public DbSet<UserNotification> Notifications { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public DbSet<UserMessage> Messages { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public DbSet<UserProfile> UserProfiles { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public DbSet<UserLogon> UserLogons { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public DbSet<Settings> Settings { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public DbSet<Forum> Forums { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public DbSet<ForumCategory> ForumCategories { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public DbSet<ForumThread> ForumThreads { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public DbSet<ForumPost> ForumPosts { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public DbSet<ForumReport> ForumReports { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public DbSet<UserKarma> UserKarma { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public DbSet<UserKarmaHistory> UserKarmaHistory { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public DbSet<SiteExpense> SiteExpenses { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public DbSet<PaytopiaItem> PaytopiaItems { get; set; }
		public DbSet<PaytopiaPayment> PaytopiaPayments { get; set; }
		public DbSet<NewsItem> NewsItem { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public DbSet<ApprovalQueue> ApprovalQueue { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public DbSet<UserVerification> UserVerification { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public DbSet<UserVerificationReject> UserVerificationReject { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public DbSet<TwoFactorCode> TwoFactorCode { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public DbSet<SupportTicket> SupportTicket { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public DbSet<SupportTicketMessage> SupportTicketMessage { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public DbSet<SupportTicketQueue> SupportTicketQueue { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public DbSet<SupportTag> SupportTag { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public DbSet<AuthenticatedFeature> AuthenticatedFeature { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public DbSet<ActivityLog> AuditLog { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public DbSet<AdminActivityLog> AdminActivityLog { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

		public void Dispose()
		{
			
		}

		public void LogActivity(string adminUserId, string activity)
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

		public Task<int> SaveChangesWithAuditAsync(string adminUserId = null)
		{
			return Task.FromResult(0);
		}
	}
}
