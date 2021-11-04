using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ExamSystem.Models
{
    public class ExamQuestion
    {
        [Key]
        [Column(Order =1)]
        public int ExamId { get; set; }
        [Key]
        [Column(Order = 2)]
        public int QuestionId { get; set; }

        [ForeignKey("ExamId")]
        public Exam Exam { get; set; }
        [ForeignKey("QuestionId")]
        public Question Question{ get; set; }

    }
}