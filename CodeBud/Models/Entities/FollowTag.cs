using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CodeBud.Models.Entities
{
    public class FollowTag
    {
        [Key]
        public int Id { get; set; }
        public int UserId { get; set; }
        public int TagId { get; set; }

        public User User { get; set; }
        public Tag Tag { get; set; }
    }
}