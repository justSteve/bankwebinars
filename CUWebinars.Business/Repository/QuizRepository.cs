using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CUWebinars.Business.Models;

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

        public Quiz GetQuizByWebinarId(int idWebinar)
        {
            return ((TTSWebinarsContext) db).Quiz.Include(
                q => q.QuizWithQuestions.Select(qwq => qwq.Question.QuestionWithOptions.Select(qwo => qwo.Option)))
                .Include(q => q.QuizWithQuestions.Select(qwq => qwq.QuizUserAnswers))
                .Include(q => q.QuizUserOrders.Select(quo => quo.Order))
                .Include(q => q.Webinar)
                .Where(q => q.idWebinar == idWebinar).FirstOrDefault();
        }
    }
}
