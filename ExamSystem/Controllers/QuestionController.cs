using ExamSystem.Models;
using ExamSystem.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ExamSystem.Controllers
{
    [Authorize]
    public class QuestionController : Controller
    {
        private ApplicationDbContext db;
        public QuestionController()
        {
            db = new ApplicationDbContext();
        }
        [HttpGet]
        public ActionResult Index()
        {
            var questions = db.Questions.ToList();
            ViewBag.examList = questions;
            return View();
        }
        [HttpGet]
        public ActionResult AddQuestion()
        {
            QuestionViewModel qvm = new QuestionViewModel();
            var exams = db.Exams.ToList();
            ViewBag.examList = new SelectList(exams, "Id", "Name");
            return PartialView("_AddQuestion");
        }
        [HttpPost]
        public ActionResult AddQuestion(QuestionViewModel qvm)
        {
            if (!ModelState.IsValid)
                return Json(new { result = 0, message = "Inserted Failed. Try Again " });
            var question = new Question()
            {
                QuestionText = qvm.QuestionText,
                Score = qvm.Score
            };
            db.Questions.Add(question);
            db.SaveChanges();
            var exam = new ExamQuestion()
            {
                ExamId = qvm.ExamId,
                QuestionId = question.Id
            };
            db.ExamQuestions.Add(exam);
            db.SaveChanges();
            var answerList = new List<Answer>();
            foreach (var ans in qvm.OptionList)
            {
                var answer = new Answer();
                if (ans == qvm.QuestionAnswer)
                    //Right Answer 
                    answer.IsCorrect = 1;
                else
                    //Wrong Answer : 
                    answer.IsCorrect = 0;

                answer.QuestionId = question.Id;
                answer.AnswerText = ans;
                answerList.Add(answer);
            }
            db.Answers.AddRange(answerList);
            db.SaveChanges();
            return Json(new { result = 1, message = "Inserted Successfully" });
        }
        [HttpGet]
        public ActionResult EditQuestion(int QuestionId)
        {
            var question = db.Questions.SingleOrDefault(x => x.Id == QuestionId);
            var exams = db.Exams.ToList();
            ViewBag.examList = new SelectList(exams, "Id", "Name");
            //get examID : 
            var examId = db.ExamQuestions.SingleOrDefault(x => x.QuestionId == QuestionId).ExamId;
            //get right answer: 
            var questionAnswer = db.Answers.SingleOrDefault(x => x.QuestionId == QuestionId && x.IsCorrect == 1).AnswerText;
            //get list of answersOptions: 
            var options = db.Answers.Where(x => x.QuestionId == QuestionId).Select(x => x.AnswerText).ToList();
            QuestionViewModel qvm = new QuestionViewModel()
            {
                QuestionText = question.QuestionText,
                Score = question.Score,
                ExamId = examId,
                QuestionAnswer = questionAnswer,
                OptionList = options
            };
            return PartialView("_EditQuestion", qvm);
        }
        [HttpPost]
        public ActionResult EditQuestion(QuestionViewModel qvm)
        {
            if (!ModelState.IsValid)
                return Json(new { result = 0, message = "Updated Failed. Try Again " });
            var question = db.Questions.SingleOrDefault(x => x.Id == qvm.QuestionId);
            question.QuestionText = qvm.QuestionText;
            question.Score = qvm.Score;
            //  db.SaveChanges();
            var newOptionsAnswers = new List<string>();
            newOptionsAnswers = qvm.OptionList;
            var oldOptionsAnswers= db.Answers.Where(x => x.QuestionId == qvm.QuestionId).ToList();
            foreach (var newOption in newOptionsAnswers.Select((newValue, i) => (newValue, i)))
            {
                var valueN = newOption.newValue;
                var indexN = newOption.i;
                oldOptionsAnswers[indexN].AnswerText = valueN;
                if(valueN == qvm.QuestionAnswer)
                    oldOptionsAnswers[indexN].IsCorrect = 1;
                else
                    oldOptionsAnswers[indexN].IsCorrect = 0;
            }
            db.SaveChanges();
            return Json(new { result = 1, message = "Inserted Successfully" });
        }
        [HttpPost]
        public ActionResult DeleteQuestion(int QuestionId)
        {
            var question = db.Questions.SingleOrDefault(x => x.Id == QuestionId);
            if (question != null)
            {
                var answers = db.Answers.Where(x => x.QuestionId == question.Id).ToList();
                var examQuestions = db.ExamQuestions.Where(x => x.QuestionId == QuestionId).ToList();
                var userQuestions = db.UserQuestions.Where(x => x.QuestionId == QuestionId).ToList();
                //Remove Answers : 
                db.Answers.RemoveRange(answers);
                //Remove ExamQuestion : 
                db.ExamQuestions.RemoveRange(examQuestions);
                //Remove Question : 
                db.Questions.Remove(question);
                //Remove UserQuestion : 
                db.UserQuestions.RemoveRange(userQuestions);
                db.SaveChanges();
                return Json(new { result = 1 });
            }
            return Json(new { result = 0 });
        }

    }
}