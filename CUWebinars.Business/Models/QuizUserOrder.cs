
namespace CUWebinars.Business.Models
{
    public partial class QuizUserOrder
    {
        public int Id { get; set; }
        public int idUser { get; set; }
        public int idOrder { get; set; }
        public int idQuiz{ get; set; }
        public Order Order { get; set; }
        public Quiz Quiz { get; set; }
        public WebUser WebUser { get; set; }
    }
}
