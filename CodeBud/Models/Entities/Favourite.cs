using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CodeBud.Models.Entities
{
    public class Favourite
    {
        [Key]
        public int Id { get; set; }
        public int UserId { get; set; }
        public int QuestionId { get; set; }

        public User User { get; set; }
        public Question Question { get; set; }
    }
}