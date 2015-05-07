using CUWebinars.Business.Models;

namespace CUWebinars.Web.Models
{
    public class QuizModel
    {
        public string Email { get; set; }
        public int OrderId { get; set; }
        public int QuestionCount { get; set; }
        public int QuizId { get; set; }
        public string QuizCode { get; set; }
        public Webinar Webinar { get; set; }
    }
}