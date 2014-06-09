using CUWebinars.Web.Models;

namespace CUWebinars.Web.ViewModel
{
    public class CreateUserConfirmedViewModel : LocalPasswordModel
    {
        public string ScreenMessage { get; set; }
        public bool UserIsLoggedIn { get; set; }
    }
}