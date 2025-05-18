using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CodeBud.Models.Entities
{
    public class UserPermission
    {
        [Key]  
        public int Id { get; set; }

        [ForeignKey("User")]
        public int UserId { get; set; }
        public UserModel User { get; set; }

        [ForeignKey("Permission")]
        public int PermissionId { get; set; }
        public Permission Permission { get; set; }
    }
}