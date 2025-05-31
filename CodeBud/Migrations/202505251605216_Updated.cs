namespace CodeBud.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Updated : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.UserPermissions", "PermissionId", "dbo.Permissions");
            DropForeignKey("dbo.UserPermissions", "UserId", "dbo.UserModels");
            DropIndex("dbo.UserPermissions", new[] { "UserId" });
            DropIndex("dbo.UserPermissions", new[] { "PermissionId" });
            AddColumn("dbo.Comments", "QuestionId", c => c.Int(nullable: false));
            AddColumn("dbo.Comments", "UserId", c => c.Int(nullable: false));
            AddColumn("dbo.UserModels", "HashedPassword", c => c.String(nullable: false));
            AddColumn("dbo.UserModels", "ImageURL", c => c.String());
            CreateIndex("dbo.Comments", "QuestionId");
            CreateIndex("dbo.Comments", "UserId");
            AddForeignKey("dbo.Comments", "QuestionId", "dbo.Questions", "Id", cascadeDelete: false);
            AddForeignKey("dbo.Comments", "UserId", "dbo.UserModels", "Id", cascadeDelete: false);
            DropColumn("dbo.UserModels", "name");
            DropColumn("dbo.UserModels", "surName");
            DropTable("dbo.Permissions");
            DropTable("dbo.UserPermissions");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.UserPermissions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.Int(nullable: false),
                        PermissionId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Permissions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.UserModels", "surName", c => c.String(nullable: false));
            AddColumn("dbo.UserModels", "name", c => c.String(nullable: false));
            DropForeignKey("dbo.Comments", "UserId", "dbo.UserModels");
            DropForeignKey("dbo.Comments", "QuestionId", "dbo.Questions");
            DropIndex("dbo.Comments", new[] { "UserId" });
            DropIndex("dbo.Comments", new[] { "QuestionId" });
            DropColumn("dbo.UserModels", "ImageURL");
            DropColumn("dbo.UserModels", "HashedPassword");
            DropColumn("dbo.Comments", "UserId");
            DropColumn("dbo.Comments", "QuestionId");
            CreateIndex("dbo.UserPermissions", "PermissionId");
            CreateIndex("dbo.UserPermissions", "UserId");
            AddForeignKey("dbo.UserPermissions", "UserId", "dbo.UserModels", "Id", cascadeDelete: true);
            AddForeignKey("dbo.UserPermissions", "PermissionId", "dbo.Permissions", "Id", cascadeDelete: true);
        }
    }
}
