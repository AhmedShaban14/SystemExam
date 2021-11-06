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
            GetListOfQuestions();
            return View();
        }
        [HttpGet]
        public ActionResult AddQuestion()
        {
            QuestionViewModel qvm = new QuestionViewModel();
            GetListOfExams();
            return PartialView("_AddQuestion");
        }
        [HttpPost]
        public ActionResult AddQuestion(QuestionViewModel qvm)
        {
            if (!ModelState.IsValid)
                return Json(new { result = 0, message = "Inserted Failed. Try Again " });
            //Add Question : 
            var question = AddQuestionToDb(qvm);
            if (question == null)
                return Json(new { result = 2, message = "Question Inserted Failed. Try Again " });
            //Add Exam : 
            if (!IsExamAddedToDb(qvm, question))
                return Json(new { result = 3, message = "Exma Inserted Failed. Try Again " });
            //Add Answers : 
            if (!IsAnswersAddedToDb(qvm, question))
                return Json(new { result = 4, message = "Answer Inserted Failed. Try Again " });
            return Json(new { result = 1, message = "Inserted Successfully" });
        }
        [HttpGet]
        public ActionResult EditQuestion(int QuestionId)
        {
            //Get Question : 
            var question = GetQuestion(QuestionId);
            //GetListOfExmas:
            GetListOfExams();
            //get examID : 
            var examId = GetExmaId(QuestionId);
            //get right answer: 
            var questionAnswer = GetQuestionAnswer(QuestionId);
            //get list of answersOptions: 
            var options = GetAnswersText(QuestionId);
           var qvm =  GetQuestionViewModel(question, examId, questionAnswer, options);
            return PartialView("_EditQuestion", qvm);
        }
        [HttpPost]
        public ActionResult EditQuestion(QuestionViewModel qvm)
        {
            if (!ModelState.IsValid)
                return Json(new { result = 0, message = "Updated Failed. Try Again " });
            //EditQuestion : 
            if (!IsQuestionUpdatedToDb(qvm))
                return Json(new { result = 2, message = "Question Updated Failed. Try Again " });
            //Edit Options In Question : 
            if (!IsOptionsOfQuestionUpdated(qvm))
                return Json(new { result = 3, message = "Options Of Question Updated Failed. Try Again " });
            db.SaveChanges();
            return Json(new { result = 1, message = "Inserted Successfully" });
        }
        [HttpPost]
        public ActionResult DeleteQuestion(int QuestionId)
        {
            var question =GetQuestion(QuestionId);
            if (question != null)
            {
                var answers =GetAnswers(question);
                var examQuestions = GetExamQuestions(QuestionId);
                var userQuestions = GetUserQuestions(QuestionId);;
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


        public Question AddQuestionToDb(QuestionViewModel qvm)
        {
            var question = new Question()
            {
                QuestionText = qvm.QuestionText,
                Score = qvm.Score
            };
            db.Questions.Add(question);
            db.SaveChanges();
            return question;
        }
        public bool IsExamAddedToDb(QuestionViewModel qvm, Question question)
        {
            if (qvm == null)
                return false;
            var exam = new ExamQuestion()
            {
                ExamId = qvm.ExamId,
                QuestionId = question.Id
            };
            db.ExamQuestions.Add(exam);
            db.SaveChanges();
            return true;
        }
        public bool IsAnswersAddedToDb(QuestionViewModel qvm, Question question)
        {
            if (qvm == null)
                return false;
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
            return true;
        }

        public bool IsQuestionUpdatedToDb(QuestionViewModel qvm)
        {
            var question = db.Questions.SingleOrDefault(x => x.Id == qvm.QuestionId);
            if (question == null)
                return false;
            question.QuestionText = qvm.QuestionText;
            question.Score = qvm.Score;
            return true;
        }
        public bool IsOptionsOfQuestionUpdated(QuestionViewModel qvm)
        {
            if (qvm == null)
                return false;
            var newOptionsAnswers = new List<string>();
            newOptionsAnswers = qvm.OptionList;
            var oldOptionsAnswers = db.Answers.Where(x => x.QuestionId == qvm.QuestionId).ToList();
            foreach (var newOption in newOptionsAnswers.Select((newValue, i) => (newValue, i)))
            {
                var valueN = newOption.newValue;
                var indexN = newOption.i;
                oldOptionsAnswers[indexN].AnswerText = valueN;
                if (valueN == qvm.QuestionAnswer)
                    oldOptionsAnswers[indexN].IsCorrect = 1;
                else
                    oldOptionsAnswers[indexN].IsCorrect = 0;
            }
            return true;
        }

        public Question GetQuestion(int QuestionId)
        {

            return db.Questions.SingleOrDefault(x => x.Id == QuestionId);
        }
        public string GetQuestionAnswer(int QuestionId)
        {
            return db.Answers.SingleOrDefault(x => x.QuestionId == QuestionId && x.IsCorrect == 1).AnswerText;
        }

        public void GetListOfQuestions()
        {
            var questions = db.Questions.ToList();
            ViewBag.examList = questions;
        }
        public void GetListOfExams()
        {
            var exams = db.Exams.ToList();
            ViewBag.examList = new SelectList(exams, "Id", "Name");
        }
        public int GetExmaId(int QuestionId)
        {
            return db.ExamQuestions.SingleOrDefault(x => x.QuestionId == QuestionId).ExamId;
        }
        public QuestionViewModel GetQuestionViewModel(Question question, int examId, string questionAnswer, List<string> options)
        {
            QuestionViewModel qvm = new QuestionViewModel()
            {
                QuestionText = question.QuestionText,
                Score = question.Score,
                ExamId = examId,
                QuestionAnswer = questionAnswer,
                OptionList = options
            };
            return qvm;
        }

        public List<string> GetAnswersText(int QuestionId)
        {
            return db.Answers.Where(x => x.QuestionId == QuestionId).Select(x => x.AnswerText).ToList();
        }
        public List<Answer> GetAnswers(Question question)
        {
            return db.Answers.Where(x => x.QuestionId == question.Id).ToList();
        }
        public List<ExamQuestion> GetExamQuestions(int QuestionId)
        {
            return db.ExamQuestions.Where(x => x.QuestionId == QuestionId).ToList();

        }
        public List<UserQuestion> GetUserQuestions(int QuestionId)
        {
            return db.UserQuestions.Where(x => x.QuestionId == QuestionId).ToList();
        }
    }
}