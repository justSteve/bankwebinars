using System.Collections.Generic;
using CUWebinars.Business.Models;

namespace CUWebinars.Web.Models
{
    public class UserQuizEditModel
    {
        public Quiz Quiz { get; set; }
        public IEnumerable<QuizQuestion> QuizQuestions { get; set; }
    }

    public class QuizQuestion
    {
        public ICollection<QuestionOption> Options { get; set; }
        public int QuestionNumber { get; set; }
        public ICollection<char> Solutions { get; set; }
        public ICollection<char> UserAnswers { get; set; }
    }

    public class QuestionOption
    {
        public string Text { get; set; }
        public char Letter { get; set; }
    }
}