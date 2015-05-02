
using System.Collections.Generic;

namespace CUWebinars.Business.Models
{
    public partial class Question
    {
        public Question()
        {
            QuestionWithOptions = new List<QuestionWithOption>();
            QuizWithQuestions = new List<QuizWithQuestion>();
        }
        public int Id { get; set; }
        public string Text { get; set; }
        public int? idTimeLimit { get; set; }
        public QuestionType QuestionType { get; set; }

        public TimeLimit TimeLimit { get; set; }
        public ICollection<QuestionWithOption> QuestionWithOptions { get; set; }
        public ICollection<QuizWithQuestion> QuizWithQuestions { get; set; }
    }
}
