namespace CodeBud.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddPermissionTablesToContext : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Permissions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.UserPermissions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.Int(nullable: false),
                        PermissionId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Permissions", t => t.PermissionId, cascadeDelete: true)
                .ForeignKey("dbo.UserModels", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.PermissionId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UserPermissions", "UserId", "dbo.UserModels");
            DropForeignKey("dbo.UserPermissions", "PermissionId", "dbo.Permissions");
            DropIndex("dbo.UserPermissions", new[] { "PermissionId" });
            DropIndex("dbo.UserPermissions", new[] { "UserId" });
            DropTable("dbo.UserPermissions");
            DropTable("dbo.Permissions");
        }
    }
}
