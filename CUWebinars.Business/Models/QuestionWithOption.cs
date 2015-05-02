
namespace CUWebinars.Business.Models
{
    public partial class QuestionWithOption
    {
        public int Id { get; set; }
        public int idQuestion { get; set; }
        public int idOption { get; set; }
        public bool CorrectAnswer { get; set; }
        public string Letter { get; set; }
        public virtual Option Option { get; set; }
        public virtual Question Question { get; set; }
    }
}
