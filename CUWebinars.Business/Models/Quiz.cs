
namespace CUWebinars.Business.Models
{
    public partial class Quiz
    {
        public int Id { get; set; }
        public int idWebinar { get; set; }
        public int? idTimeLimit { get; set; }

        public TimeLimit TimeLimit { get; set; }
        public Webinar Webinar { get; set; }
    }
}
