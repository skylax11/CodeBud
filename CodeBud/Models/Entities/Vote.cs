using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CodeBud.Models.Entities
{
    public class Vote
    {
        [Key]
        public int Id { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }

        [Required]
        public string TargetType { get; set; }  // "question", "answer"

        public int TargetId { get; set; }

        [Range(-1, 1)]
        public int VoteType { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}