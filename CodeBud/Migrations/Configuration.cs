namespace CodeBud.Migrations
{
    using CodeBud.Models.Entities;
    using CodeBud.Models.Enums;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<CodeBud.DbContext.AppDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(CodeBud.DbContext.AppDbContext context)
        {
            foreach (var perm in Enum.GetValues(typeof(PermissionType)).Cast<PermissionType>())
            {
                var name = perm.ToString();
                if (!context.Permissions.Any(p => p.Name == name))
                {
                    context.Permissions.Add(new Permission { Name = name });
                }
            }
            context.SaveChanges();
        }
    }
}
