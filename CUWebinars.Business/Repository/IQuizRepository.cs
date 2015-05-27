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
        Quiz GetQuizByIdWithOptionsText(int id);
        Quiz GetQuizByIdWithOptionsAndUserAnswers(int id);
        Quiz GetQuizByWebinarId(int idWebinar);
        Quiz GetQuizByCode(string quizCode);
        Quiz GetQuizFromOrder(int idOrder);
        int GetQuizIdByWebinarId(int idWebinar);
        QuestionCountAndWebinarId GetQuestionCountAndWebinarId(string quizCode);
        void SetQuizUserAnswerAsModified(QuizUserAnswer quizUserAnswer);
        void DeleteQuizWithQuestion(QuizWithQuestion quizWithQuestion);
        void DeleteQuizWithOption(QuestionWithOption questionWithOption);

        TTSWebinarsContext Context { get; }
        int CloneQuiz(Quiz quiz, int idWebinar);
    }
}