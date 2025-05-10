using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CodeBud.Models.Entities
{
    public class Tag
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(50)]
        public string Name { get; set; }

        public string Description { get; set; }
        public int UsageCount { get; set; } = 0;

        public ICollection<QuestionTag> QuestionTags { get; set; }
    }
}