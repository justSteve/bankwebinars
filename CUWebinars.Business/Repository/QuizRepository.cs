using System;
using System.Collections.Generic;
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

        public void AddQuizWithQuestion(QuizWithQuestion quizWithQuestion)
        {
            throw new NotImplementedException();
        }

        public void SaveChanges()
        {
            db.SaveChanges();
        }
    }
}
