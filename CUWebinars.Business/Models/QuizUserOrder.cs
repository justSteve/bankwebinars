
namespace CUWebinars.Business.Models
{
    public partial class QuizUserOrder
    {
        public int Id { get; set; }
        public int idUser { get; set; }
        public int idOrder { get; set; }
        public int idQuiz { get; set; }
        public string Email { get; set; }
        public virtual Order Order { get; set; }
        public virtual Quiz Quiz { get; set; }
    }
}
