
namespace CUWebinars.Business.Models
{
    public partial class QuizUserAnswer
    {
        public int Id { get; set; }
        public int idQuizQuestion { get; set; }
        public string Email { get; set; }
        public string Letter { get; set; }
        public virtual QuizWithQuestion QuizWithQuestion { get; set; }
    }
}
