
namespace CUWebinars.Business.Models
{
    public partial class QuizUserAnswer
    {
        public int Id { get; set; }
        public int idUser { get; set; }
        public int idQuizQuestion { get; set; }
        public char Answer { get; set; }

        public QuizWithQuestion QuizWithQuestion { get; set; }
        public WebUser WebUser { get; set; }
    }
}
