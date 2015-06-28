using System.Collections.Generic;
using System.Collections.ObjectModel;
using CUWebinars.Business.Core.Helpers;
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
            ((TTSWebinarsContext) db).Question.Attach(question);
            //((TTSWebinarsContext) db).Question.Add(question);
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
                .Include(q => q.QuizWithQuestions.Select(qwq => qwq.QuizUserAnswers))
                .SingleOrDefault(q => q.Id == id);
            return quiz;
        }

        public Quiz GetQuizByIdWithOptionsText(int id)
        {
            var quiz = items.Include(q => q.QuizWithQuestions.Select(qwq => qwq.Question.QuestionWithOptions.Select(qwo => qwo.Option)))
                .Include(q => q.QuizWithQuestions.Select(qwq => qwq.QuizUserAnswers))
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

        public int GetQuizIdByWebinarId(int idWebinar)
        {
            return items.Where(q => q.idWebinar == idWebinar).Select(q => q.Id).Single();
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

        public void SetQuizUserAnswerAsModified(QuizUserAnswer quizUserAnswer)
        {
            db.Entry(quizUserAnswer).State = EntityState.Modified;
        }

        public void DeleteQuizWithQuestion(QuizWithQuestion quizWithQuestion)
        {
            db.Entry(quizWithQuestion).State = EntityState.Deleted;
        }

        public void DeleteQuizWithOption(QuestionWithOption questionWithOption)
        {
            db.Entry(questionWithOption).State = EntityState.Deleted;
        }

        public TTSWebinarsContext Context
        {
            get { return db as TTSWebinarsContext; } 
        }

        public int CloneQuiz(Quiz quiz, int idWebinar)
        {
            var newQuiz = new Quiz
            {
                idWebinar = idWebinar,
                QuizWithQuestions = new List<QuizWithQuestion>(),
                QuizCode = RandomHelpers.GetUniqueCode(6)
            };

            foreach (var quizWithQuestion in quiz.QuizWithQuestions)
            {
                IList<QuestionWithOption> questionWithOptions = new List<QuestionWithOption>();

                foreach (var questionWithOption in quizWithQuestion.Question.QuestionWithOptions)
                {
                    questionWithOptions.Add(new QuestionWithOption
                    {
                        CorrectAnswer = questionWithOption.CorrectAnswer,
                        Letter = questionWithOption.Letter,
                        Option = new Option
                        {
                         Text = questionWithOption.Option.Text   
                        }
                    });
                }

                newQuiz.QuizWithQuestions.Add(new QuizWithQuestion
                {
                    QuestionNumber = quizWithQuestion.QuestionNumber,
                    Question = new Question
                    {
                        idType = QuestionType.MultipleChoice,
                        QuestionWithOptions = questionWithOptions,
                        Text = quizWithQuestion.Question.Text
                    }
                });
            }

            items.Add(newQuiz);

            db.SaveChanges();

            return newQuiz.Id;
        }

        public Quiz GetQuizFromOrder(int idOrder)
        {
            var context = (TTSWebinarsContext) db;

            //var orderRow = context.OrderRows.Single(orow => orow.idOrder == idOrder);

            //var webinar = context.Webinars.Single(w => w.idWebinar == orderRow.idWebinar);

            //var quiz = items.SingleOrDefault(q => q.idWebinar == webinar.idWebinar);
            var quiz = items.SingleOrDefault(
                q => q.idWebinar == context.Webinars.Single(
                    webinar => webinar.idWebinar == context.OrderRows.Single(
                        orderRow => orderRow.idOrder == idOrder).idWebinar).idWebinar
                        );

            return quiz;
        }
    }
}
