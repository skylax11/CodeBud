using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CodeBud.Models.Entities
{
    public class ExternalAccount
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Provider { get; set; }

        [Required]
        public string ProviderId { get; set; }

        public string Email { get; set; }
        public string AvatarUrl { get; set; }

        [ForeignKey("User")]
        public int UserId { get; set; }
        public User User { get; set; }
    }
}