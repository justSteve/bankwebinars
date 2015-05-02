using CUWebinars.Business.Models;

namespace CUWebinars.Business.Repository
{
    public interface IQuizRepository
    {
        void AddQuestion(Question question);
        void AddQuiz(Quiz quiz);
        void AddQuestionWithOption(QuestionWithOption questionWithOption);
        void AddQuizWithQuestion(QuizWithQuestion quizWithQuestion);
        void SaveChanges();
    }
}