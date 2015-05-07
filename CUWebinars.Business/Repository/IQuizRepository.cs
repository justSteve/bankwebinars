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
        Quiz GetQuizByIdWithOptions(int id);
        Quiz GetQuizByWebinarId(int idWebinar);
        Quiz GetQuizByCode(string quizCode);
        QuestionCountAndWebinarId GetQuestionCountAndWebinarId(string quizCode);
    }
}