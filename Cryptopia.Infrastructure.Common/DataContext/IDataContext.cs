using System;
using System.Data.Entity;
using System.Threading.Tasks;
using Cryptopia.Entity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Cryptopia.Infrastructure.Common.DataContext
{
    public interface IDataContext : IDisposable
    {
        Database Database { get; }
        int SaveChanges();
        Task<int> SaveChangesAsync();

        Task<int> SaveChangesWithAuditAsync(string adminUserId = null);

        DbSet<ApplicationUser> Users { get; set; }
        DbSet<IdentityRole> Roles { get; set; }
        DbSet<UserRole> UserRoles { get; set; }
        DbSet<ChatMessage> ChatMessages { get; set; }
        DbSet<UserSettings> UserSettings { get; set; }
        DbSet<UserTwoFactor> UserTwoFactor { get; set; }
        DbSet<EmailTemplate> EmailTemplates { get; set; }
        DbSet<UserNotification> Notifications { get; set; }
        DbSet<UserMessage> Messages { get; set; }
        DbSet<UserProfile> UserProfiles { get; set; }
        DbSet<UserLogon> UserLogons { get; set; }
        DbSet<Settings> Settings { get; set; }
        DbSet<Forum> Forums { get; set; }
        DbSet<ForumCategory> ForumCategories { get; set; }
        DbSet<ForumThread> ForumThreads { get; set; }
        DbSet<ForumPost> ForumPosts { get; set; }
        DbSet<ForumReport> ForumReports { get; set; }
        DbSet<UserKarma> UserKarma { get; set; }
        DbSet<UserKarmaHistory> UserKarmaHistory { get; set; }
        DbSet<SiteExpense> SiteExpenses { get; set; }
        DbSet<PaytopiaItem> PaytopiaItems { get; set; }
        DbSet<PaytopiaPayment> PaytopiaPayments { get; set; }

        DbSet<NewsItem> NewsItem { get; set; }
        DbSet<ApprovalQueue> ApprovalQueue { get; set; }
        DbSet<Entity.UserVerification> UserVerification { get; set; }
        DbSet<Entity.UserVerificationReject> UserVerificationReject { get; set; }
        DbSet<TwoFactorCode> TwoFactorCode { get; set; }
        DbSet<Entity.Support.SupportTicket> SupportTicket { get; set; }
        DbSet<Entity.Support.SupportTicketMessage> SupportTicketMessage { get; set; }
        DbSet<Entity.Support.SupportTicketQueue> SupportTicketQueue { get; set; }
        DbSet<Entity.Support.SupportTag> SupportTag { get; set; }
        DbSet<AuthenticatedFeature> AuthenticatedFeature { get; set; }
        DbSet<ActivityLog> AuditLog { get; set; }
        DbSet<AdminActivityLog> AdminActivityLog { get; set; }

        void LogActivity(string adminUserId, string activity);
	}
}