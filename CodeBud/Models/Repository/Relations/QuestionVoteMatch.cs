using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CodeBud.Models.Repository.Relations
{
    public class QuestionVoteMatch
    {
        [Key]
        public int Id { get; set; }
        public int QuestionId { get; set; }
        public int VoteId { get; set; }
    }
}