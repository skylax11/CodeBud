using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity.Migrations.Model;
using System.Linq;
using System.Web;

namespace CodeBud.Models.Entities
{
    public class UserModel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string name { get; set; }

        [Required]
        public string surName { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public string Role { get; set; }

        public virtual ICollection<UserPermission> Permissions { get; set; }

        // İLİŞKİLER İÇİN AYRICA CLASSLAR EKLENECEK.
    }
}