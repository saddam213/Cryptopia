namespace Web.Site.Migrations
{
	using System.Data.Entity.Migrations;

	public partial class Forums : DbMigration
    {
        public override void Up()
        {
        
            CreateTable(
                "dbo.ForumPost",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ThreadId = c.Int(nullable: false),
                        Message = c.String(),
                        UserId = c.String(nullable: false, maxLength: 128),
                        IsFirstPost = c.Boolean(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        LastUpdate = c.DateTime(nullable: false),
                        Timestamp = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ForumThread", t => t.ThreadId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.ThreadId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.ForumThread",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CategoryId = c.Int(nullable: false),
                        Title = c.String(maxLength: 256),
                        Description = c.String(maxLength: 512),
                        UserId = c.String(nullable: false, maxLength: 128),
                        Icon = c.String(maxLength: 128),
                        IsPinned = c.Boolean(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        LastUpdate = c.DateTime(nullable: false),
                        Timestamp = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ForumCategory", t => t.CategoryId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.CategoryId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.ForumCategory",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ForumId = c.Int(nullable: false),
                        Name = c.String(),
                        Order = c.Int(nullable: false),
                        Description = c.String(maxLength: 512),
                        Icon = c.String(maxLength: 128),
                        IsDeleted = c.Boolean(nullable: false),
                        Timestamp = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Forum", t => t.ForumId, cascadeDelete: true)
                .Index(t => t.ForumId);
            
            CreateTable(
                "dbo.Forum",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Title = c.String(maxLength: 128),
                        Description = c.String(maxLength: 512),
                        Order = c.Int(nullable: false),
                        Icon = c.String(maxLength: 128),
                        IsDeleted = c.Boolean(nullable: false),
                        Timestamp = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
          
            
            CreateTable(
                "dbo.ForumReport",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(maxLength: 128),
                        PostId = c.Int(nullable: false),
                        Message = c.String(maxLength: 1000),
                        Timestamp = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ForumPost", t => t.PostId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.UserId)
                .Index(t => t.PostId);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ForumReport", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.ForumReport", "PostId", "dbo.ForumPost");
            DropForeignKey("dbo.ForumPost", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.ForumThread", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.ForumPost", "ThreadId", "dbo.ForumThread");
            DropForeignKey("dbo.ForumThread", "CategoryId", "dbo.ForumCategory");
            DropForeignKey("dbo.ForumCategory", "ForumId", "dbo.Forum");
            DropIndex("dbo.ForumReport", new[] { "PostId" });
            DropIndex("dbo.ForumReport", new[] { "UserId" });
            DropIndex("dbo.ForumCategory", new[] { "ForumId" });
            DropIndex("dbo.ForumThread", new[] { "UserId" });
            DropIndex("dbo.ForumThread", new[] { "CategoryId" });
            DropIndex("dbo.ForumPost", new[] { "UserId" });
            DropIndex("dbo.ForumPost", new[] { "ThreadId" });
            DropTable("dbo.ForumReport");
            DropTable("dbo.Forum");
            DropTable("dbo.ForumCategory");
            DropTable("dbo.ForumThread");
            DropTable("dbo.ForumPost");
        }
    }
}
