
using System.Collections.Generic;

namespace CUWebinars.Business.Models
{
    public partial class Quiz
    {
        public Quiz()
        {
            this.QuizUserOrders = new List<QuizUserOrder>();
            this.QuizWithQuestions = new List<QuizWithQuestion>();
        }

        public int Id { get; set; }
        public int idWebinar { get; set; }
        public int? idTimeLimit { get; set; }

        public virtual Webinar Webinar { get; set; }
        public virtual ICollection<QuizUserOrder> QuizUserOrders { get; set; }
        public virtual ICollection<QuizWithQuestion> QuizWithQuestions { get; set; }
    }
}
