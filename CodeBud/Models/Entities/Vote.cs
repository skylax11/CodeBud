using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CodeBud.Models.Entities
{
    public class Vote
    {
        [Key]
        public int Id { get; set; }

        public bool IsUpvote { get; set; }

        [ForeignKey("User")]
        public int UserId { get; set; }
        public UserModel User { get; set; }

        [ForeignKey("Question")]
        public int? QuestionId { get; set; }
        public Question Question { get; set; }

        [ForeignKey("Answer")]
        public int? AnswerId { get; set; }
        public Answer Answer { get; set; }
    }
}