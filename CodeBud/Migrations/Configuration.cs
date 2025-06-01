namespace CodeBud.Migrations
{
    using CodeBud.Models.Entities;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<CodeBud.DbContext.AppDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(CodeBud.DbContext.AppDbContext context)
        {
            // 1. Enum'dan tüm izinleri al
            var permissionNames = Enum.GetNames(typeof(CodeBud.Models.Enums.PermissionType));

            // 2. Permission tablosuna eksik olanları ekle
            foreach (var name in permissionNames)
            {
                if (!context.Permissions.Any(p => p.Name == name))
                {
                    context.Permissions.Add(new Permission { Name = name });
                }
            }

            context.SaveChanges();

            // 3. Admin kullanıcıyı bul (rolü "Admin" olan ilk kullanıcı)
            var admin = context.Users.FirstOrDefault(u => u.Role == "Admin");

            // 4. Eğer admin kullanıcı varsa, tüm izinleri ona ata
            if (admin != null)
            {
                var allPermissions = context.Permissions.ToList();

                foreach (var perm in allPermissions)
                {
                    bool alreadyAssigned = context.UserPermissions
                        .Any(up => up.UserId == admin.Id && up.PermissionId == perm.Id);

                    if (!alreadyAssigned)
                    {
                        context.UserPermissions.Add(new UserPermission
                        {
                            UserId = admin.Id,
                            PermissionId = perm.Id
                        });
                    }
                }

                context.SaveChanges();
            }
        }
    }
}
