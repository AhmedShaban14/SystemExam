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
            GetListOfExams();
            return View(uq);
        }
        [HttpGet]
        public ActionResult ShowResult()
        {
            UserQuestion uq = new UserQuestion();
            GetListOfExams();
            return View(uq);
        }
        [HttpGet]
        public ActionResult ShowQuestion(int examId)
        {
            UserQuestionViewModel uqvm = new UserQuestionViewModel();
            uqvm.UserId = 2;
            uqvm.ExamId = examId;
            var questionIdsList = GetListOfQuestionIds(examId);
            var questionList = GetQuestions(questionIdsList);
            var randomQuestionList = MakeRandomQuestions(questionList);
            var answerList = GetAnswers(randomQuestionList);
            ViewBag.questionList = randomQuestionList;
            ViewBag.answerList = answerList;
            return View(uqvm);
        }
        [HttpPost]
        public ActionResult ShowQuestion(UserQuestionViewModel uqvm)
        {
            if (!ModelState.IsValid)
                return Json(new { result = 0 });
            var answer = GetRightAnswer(uqvm);
            if (answer == null)
                return Json(new { result = 0 });
            var answerId = answer.Id;
            if (!IsUserQuestionAdded(uqvm, answerId))
                return Json(new { result = 3 });
            return Json(new { result = 1 });
        }
        [HttpGet]
        public ActionResult ShowExamsForUser(int examId)
        {
            var userQuestions = GetUserExam(examId);
            if (userQuestions == null)
                return Json(new { result = 0 }, JsonRequestBehavior.AllowGet);
            var questionsIds = GetQuestionIds(userQuestions);
            var answerTextList = new List<Answer>();
            var answerQuestionList = new List<Question>();
            var totalScore = CalcutalteUserScore(questionsIds,out answerTextList,out answerQuestionList);
            var urvm = CreateUserResultViewModel(totalScore,answerTextList,answerQuestionList);
            return View(urvm);
        }

        public void GetListOfExams()
        {
            var exams = db.Exams.ToList();
            ViewBag.examList = new SelectList(exams, "Id", "Name");
        }
        public Answer GetRightAnswer(UserQuestionViewModel uqvm)
        {
            return db.Answers.SingleOrDefault(x => x.AnswerText == uqvm.AnswerText && x.QuestionId == uqvm.QuestionId);
        }
        public bool IsUserQuestionAdded(UserQuestionViewModel uqvm, int answerId)
        {
            if (uqvm == null || answerId == 0)
                return false;
            var userQuestion = new UserQuestion()
            {
                UserId = uqvm.UserId,
                ExamId = uqvm.ExamId,
                QuestionId = uqvm.QuestionId,
                AnswerId = answerId
            };
            db.UserQuestions.Add(userQuestion);
            db.SaveChanges();
            return true;
        }
        public IEnumerable<int> GetListOfQuestionIds(int examId)
        {
            return db.ExamQuestions.Where(x => x.ExamId == examId).ToList().Select(x => x.QuestionId);
        }
        public List<Question> GetQuestions(IEnumerable<int> questionIdsList)
        {
            var questionList = new List<Question>();
            foreach (var questionId in questionIdsList)
            {
                var question = db.Questions.SingleOrDefault(x => x.Id == questionId);
                questionList.Add(question);
            }
            return questionList;
        }
        public List<Question> MakeRandomQuestions(List<Question> questionList)
        {
            Random random = new Random();
            //Make Question List Rndom:
            return questionList.OrderBy(x => random.Next()).Take(10).ToList();
        }
        public List<Answer> GetAnswers(List<Question> randomQuestionList)
        {
            var answerList = new List<Answer>();
            foreach (var ques in randomQuestionList)
            {
                var answer = db.Answers.Where(x => x.QuestionId == ques.Id).ToList();
                answerList.AddRange(answer);
            }
            return answerList;
        }
        public List<UserQuestion> GetUserExam(int examId)
        {
            return db.UserQuestions.Where(x => x.ExamId == examId && x.UserId == 2).ToList();
        }
        public IEnumerable<int> GetQuestionIds(List<UserQuestion> userQuestions)
        {
            return userQuestions.Select(x => x.QuestionId);
        }
        public int CalcutalteUserScore(IEnumerable<int> questionsIds, out List<Answer> answerTextList, out List<Question> answerQuestionList)
        {
            var totalScore = 0;
            answerTextList = new List<Answer>();
            answerQuestionList = new List<Question>();
            foreach (int quesId in questionsIds)
            {
                var question = db.Questions.SingleOrDefault(x => x.Id == quesId);
                var answerTexts = db.Answers.Where(x => x.QuestionId == quesId);
                totalScore += question.Score;
                answerTextList.AddRange(answerTexts);
                answerQuestionList.Add(question);
            }
            return totalScore;
        }
        public UserResultViewModel CreateUserResultViewModel(int totalScore, List<Answer> answerTextList, List<Question> answerQuestionList)
        {
            UserResultViewModel urvm = new UserResultViewModel();
            urvm.userEmail = db.UserInfos.SingleOrDefault(x => x.Id == 2).Email;
            urvm.Total = totalScore;
            urvm.userAnswerList = answerTextList;
            urvm.userQuestionList = answerQuestionList;
            return urvm;
        }
    }
}