
namespace CUWebinars.Business.Models
{
    public partial class QuizWithQuestion
    {
        public int Id { get; set; }

        public int idQuiz { get; set; }
        public int idQuestion { get; set; }
        public int QuestionNumber { get; set; }

        public Quiz Quiz { get; set; }
    }
}
