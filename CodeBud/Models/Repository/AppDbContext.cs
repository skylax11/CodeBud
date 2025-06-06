using CodeBud.Models;
using System.Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CodeBud.Models.Entities;
using CodeBud.Models.Repository.Relations;

namespace CodeBud.DbContext
{
    public class AppDbContext : System.Data.Entity.DbContext
    {
        public DbSet<UserModel> Users { get; set; }
        public DbSet<UserPermission> UserPermissions { get; set; }
        public DbSet<Answer> Answers { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<Vote> Votes { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<QuestionTagMatch> QuestionTags { get; set; }
        public DbSet<QuestionVoteMatch> QuestionVote { get; set; }
        public DbSet<Permission> Permissions { get; set; }
    }
}