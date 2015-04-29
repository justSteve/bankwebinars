
namespace CUWebinars.Business.Models
{
    public partial class Question
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public int? idTimeLimit { get; set; }
        public QuestionType QuestionType { get; set; }

        public TimeLimit TimeLimit { get; set; }

    }
}
