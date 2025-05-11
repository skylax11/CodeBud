using CodeBud.Models;
using System.Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CodeBud.DbContext
{
    public class AppDbContext : System.Data.Entity.DbContext
    {
        public AppDbContext() : base("DefaultConnection") { }

        public DbSet<UserModel> Users { get; set; }
    }
}