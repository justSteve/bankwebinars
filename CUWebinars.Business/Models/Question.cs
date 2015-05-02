
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
        public QuestionType idType { get; set; }

        public virtual ICollection<QuestionWithOption> QuestionWithOptions { get; set; }
        public virtual ICollection<QuizWithQuestion> QuizWithQuestions { get; set; }
    }
}
