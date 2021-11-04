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
    public class UserController : Controller
    {
      private ApplicationDbContext db;
        public UserController()
        {
            db = new ApplicationDbContext();
        }
        [HttpGet]
        public ActionResult Index()
        {
            UserQuestion uq = new UserQuestion();
            var exams = db.Exams.ToList();
            ViewBag.examList = new SelectList(exams, "Id", "Name");
            return View(uq);
        }
        [HttpGet]
        public ActionResult ShowResult()
        {
            UserQuestion uq = new UserQuestion();
            var exams = db.Exams.ToList();
            ViewBag.examList = new SelectList(exams, "Id", "Name");
            return View(uq);
        }
        [HttpGet]
        public ActionResult ShowQuestion(int examId)
        {
            UserQuestionViewModel uqvm = new UserQuestionViewModel();
            uqvm.UserId = 2;
            uqvm.ExamId = examId;
            var questionIdsList = db.ExamQuestions.Where(x => x.ExamId == examId).ToList().Select(x => x.QuestionId);
            var questionList = new List<Question>();
            var answerList = new List<Answer>();
            Random random = new Random();
            foreach (var questionId in questionIdsList)
            {
                var question = db.Questions.SingleOrDefault(x => x.Id == questionId);
                questionList.Add(question);
            }
            //Make Question List Rndom:
            var randomQuestionList = questionList.OrderBy(x => random.Next()).Take(10).ToList();
            foreach (var ques in randomQuestionList)
            {
                var answer = db.Answers.Where(x => x.QuestionId == ques.Id).ToList();
                answerList.AddRange(answer);
            }
            ViewBag.questionList = randomQuestionList;
            ViewBag.answerList = answerList;
            return View(uqvm);
        }
        [HttpPost]
        public ActionResult ShowQuestion(UserQuestionViewModel uqvm)
        {
            if (!ModelState.IsValid)
                return Json(new { result = 0 });
            var answer = db.Answers.SingleOrDefault(x => x.AnswerText == uqvm.AnswerText && x.QuestionId == uqvm.QuestionId);
            if (answer == null)
                return Json(new { result = 0 });
            var answerId = answer.Id;
            var userQuestion = new UserQuestion()
            {
                UserId = uqvm.UserId,
                ExamId = uqvm.ExamId,
                QuestionId = uqvm.QuestionId,
                AnswerId = answerId
            };
            db.UserQuestions.Add(userQuestion);
            db.SaveChanges();
            return Json(new { result = 1 });
        }
        [HttpGet]
        public ActionResult ShowExamsForUser(int examId)
        {
            UserResultViewModel urvm = new UserResultViewModel();
            var userExam = db.UserQuestions.Where(x => x.ExamId == examId && x.UserId == 2);
            if (userExam == null)
                return Json(new { result = 2 }, JsonRequestBehavior.AllowGet);
            var userQuestions = db.UserQuestions.Where(x => x.UserId == 2 && x.ExamId == examId).ToList();
            if (userQuestions == null)
                return Json(new { result = 0 }, JsonRequestBehavior.AllowGet);
            var questionsIds = userQuestions.Select(x => x.QuestionId);
            var totalScore = 0;
            var answerTextList = new List<Answer>();
            var answerQuestionList = new List<Question>();
            foreach (int quesId in questionsIds)
            {
                var question = db.Questions.SingleOrDefault(x => x.Id == quesId);
                var answerTexts = db.Answers.Where(x => x.QuestionId == quesId);
                totalScore += question.Score;
                answerTextList.AddRange(answerTexts);
                answerQuestionList.Add(question);
            }
            urvm.userEmail = db.UserInfos.SingleOrDefault(x => x.Id == 2).Email;
            urvm.Total = totalScore;
            urvm.userAnswerList = answerTextList;
            urvm.userQuestionList = answerQuestionList;
            return View(urvm);
        }
    }
}