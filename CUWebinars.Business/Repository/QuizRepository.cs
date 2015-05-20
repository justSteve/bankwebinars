using CUWebinars.Business.Models;
using System.Data.Entity;
using System.Linq;

namespace CUWebinars.Business.Repository
{
    public class QuizRepository : TTSWebinarsRepository<TTSWebinarsContext, Quiz>, IQuizRepository
    {
        public QuizRepository()
        {
            
        }

        public QuizRepository(TTSWebinarsContext context)
            : base(context)
        {

        }

        public void AddQuestion(Question question)
        {
            ((TTSWebinarsContext) db).Question.Add(question);
        }

        public void AddQuiz(Quiz quiz)
        {
            items.Add(quiz);
        }

        public void AddQuestionWithOption(QuestionWithOption questionWithOption)
        {
            ((TTSWebinarsContext) db).QuestionWithOption.Add(questionWithOption);
        }

        public void AddQuizWithQuestion(QuizWithQuestion quizWithQuestion)
        {
            ((TTSWebinarsContext) db).QuizWithQuestion.Add(quizWithQuestion);
        }

        public void SaveChanges()
        {
            db.SaveChanges();
        }

        public Quiz GetQuizByIdWithOptions(int id)
        {
            var quiz = items.Include(q => q.QuizWithQuestions.Select(qwq => qwq.Question.QuestionWithOptions))
                .SingleOrDefault(q => q.Id == id);
            return quiz;
        }

        public Quiz GetQuizByIdWithOptionsAndUserAnswers(int id)
        {
            var quiz = items.Include(q => q.QuizWithQuestions.Select(qwq => qwq.Question.QuestionWithOptions))
                .Include(q => q.QuizWithQuestions.Select(qwq => qwq.QuizUserAnswers))
                .Include(q => q.QuizUserOrders)
                .SingleOrDefault(q => q.Id == id);
            return quiz;
        }

        public Quiz GetQuizByWebinarId(int idWebinar)
        {
            return items.Include(
                q => q.QuizWithQuestions.Select(qwq => qwq.Question.QuestionWithOptions.Select(qwo => qwo.Option)))
                .Include(q => q.QuizWithQuestions.Select(qwq => qwq.QuizUserAnswers))
                .Include(q => q.QuizUserOrders.Select(quo => quo.Order))
                .Include(q => q.Webinar)
                .Where(q => q.idWebinar == idWebinar).FirstOrDefault();
        }

        public Quiz GetQuizByCode(string quizCode)
        {
            return items.Include(
                q => q.QuizWithQuestions.Select(qwq => qwq.Question.QuestionWithOptions.Select(qwo => qwo.Option)))
                .Include(q => q.QuizWithQuestions.Select(qwq => qwq.QuizUserAnswers))
                .Include(q => q.QuizUserOrders.Select(quo => quo.Order))
                .Include(q => q.Webinar)
                .Where(q => q.QuizCode == quizCode).FirstOrDefault();
        }

        public QuestionCountAndWebinarId GetQuestionCountAndWebinarId(string quizCode)
        {
            var quiz = items.Include(q => q.QuizWithQuestions).Where(q => q.QuizCode == quizCode).First();

            return new QuestionCountAndWebinarId
            {
                QuizId = quiz.Id,
                QuestionCount = quiz.QuizWithQuestions.Count,
                WebinarId = quiz.idWebinar
            };
        }
    }
}
