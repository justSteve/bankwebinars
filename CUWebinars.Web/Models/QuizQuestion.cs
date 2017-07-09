using System.Collections.Generic;

namespace CUWebinars.Web.Models
{
    public class QuizQuestion
    {
        public ICollection<QuestionOption> Options { get; set; }
        public int QuestionId { get; set; }
        public int QuestionNumber { get; set; }
        public string QuestionText { get; set; }
        public ICollection<char> Solutions { get; set; }
        public ICollection<char> UserAnswers { get; set; }
    }
}