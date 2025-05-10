using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CodeBud.Models.Entities
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(50)]
        public string Username { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        [MaxLength(256)]
        public string PasswordHash { get; set; }

        public int Reputation { get; set; } = 0;
        public bool IsModerator { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public string Bio { get; set; }
        public string AvatarUrl { get; set; }
        public string Location { get; set; }

        public ICollection<ExternalAccount> ExternalAccounts { get; set; }
        public ICollection<Question> Questions { get; set; }
        public ICollection<Answer> Answers { get; set; }
    }
}