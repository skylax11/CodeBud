using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CodeBud.Models.Entities
{
    public class Answer
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Content { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public bool IsAccepted { get; set; } = false;

        public int QuestionId { get; set; }
        public Question Question { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }

        public ICollection<Comment> Comments { get; set; }
    }
}