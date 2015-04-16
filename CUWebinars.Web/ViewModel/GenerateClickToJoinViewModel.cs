using System.ComponentModel.DataAnnotations;

namespace CUWebinars.Web.ViewModel
{
    public class GenerateClickToJoinViewModel
    {
        public string Email { get; set; }

        public int? WebinarId { get; set; }

        public int? OrderId { get; set; }
    }
}