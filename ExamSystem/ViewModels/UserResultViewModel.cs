using ExamSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ExamSystem.ViewModels
{
    public class UserResultViewModel
    {
        public string userEmail{ get; set; }
        public List<Answer> userAnswerList { get; set; }
        public List<Question> userQuestionList { get; set; }
        public int Total { get; set; }


    }
}