using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CodeBud.Models.Entities
{
    public class Question
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(200)]
        public string Title { get; set; }

        [Required]
        public string Content { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        public int ViewsCount { get; set; } = 0;
        public bool IsClosed { get; set; } = false;

        public int UserId { get; set; }
        public User User { get; set; }

        public ICollection<Answer> Answers { get; set; }
        public ICollection<QuestionTag> QuestionTags { get; set; }
        public ICollection<Comment> Comments { get; set; }
    }
}