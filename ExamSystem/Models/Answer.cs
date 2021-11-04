using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ExamSystem.Models
{
    public class Answer
    {
        [Key]
        public int Id { get; set; }
        public int QuestionId { get; set; }
        public string AnswerText { get; set; }
        public byte IsCorrect { get; set; }
        [ForeignKey("QuestionId")]
        public Question Question { get; set; }
    }
}