
using System.Collections.Generic;

namespace CUWebinars.Business.Models
{
    public partial class QuizWithQuestion
    {
        public QuizWithQuestion()
        {
            this.QuizUserAnswers = new List<QuizUserAnswer>();
        }

        public int Id { get; set; }
        public int idQuiz { get; set; }
        public int idQuestion { get; set; }
        public int QuestionNumber { get; set; }
        public virtual Question Question { get; set; }
        public virtual Quiz Quiz { get; set; }
        public virtual ICollection<QuizUserAnswer> QuizUserAnswers { get; set; }
    }
}
