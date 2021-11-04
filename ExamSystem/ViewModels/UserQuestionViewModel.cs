using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ExamSystem.ViewModels
{
    public class UserQuestionViewModel
    {
        public int UserId { get; set; }
        public int ExamId { get; set; }
        public string AnswerText{ get; set; }
        public int QuestionId { get; set; }
        //public List<String> OptionList { get; set; }

    }
}