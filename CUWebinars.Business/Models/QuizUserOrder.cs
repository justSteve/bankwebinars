
using System;

namespace CUWebinars.Business.Models
{
    public partial class QuizUserOrder
    {
        public int Id { get; set; }
        public int idOrder { get; set; }
        public int idQuiz { get; set; }
        public string Email { get; set; }
        public int QuestionCount { get; set; }
        public int Score { get; set; }
        public DateTime DateQuizTaken { get; set; }
        public virtual Order Order { get; set; }
        public virtual Quiz Quiz { get; set; }
    }
}
