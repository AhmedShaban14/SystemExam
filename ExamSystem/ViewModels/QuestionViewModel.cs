using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ExamSystem.ViewModels
{
    public class QuestionViewModel
    {
        public int ExamId { get; set; }
        public int QuestionId{ get; set; }
        public string QuestionText { get; set; }
        public int Score { get; set; }
        public string QuestionAnswer { get; set; }
        public List<String> OptionList{ get; set; }

    }
}