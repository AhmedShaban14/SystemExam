using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ExamSystem.Models
{
    public class UserQuestion
    {
        [Key]
        public int Id { get; set; }
        public int UserId { get; set; }
        public int ExamId { get; set; }
        public int AnswerId { get; set; }
        public int QuestionId { get; set; }


        [ForeignKey("UserId")]
        public UserInfo UserInfo{ get; set; }

        [ForeignKey("ExamId")]
        public Exam Exam { get; set; }

        [ForeignKey("AnswerId")]
        public Answer Answer{ get; set; }

        [ForeignKey("QuestionId")]
        public Question Question{ get; set; }
    }
}