using CUWebinars.Web.ViewModel;

namespace CUWebinars.Web.Models
{
    public class LoginModel
    {
        public string ActiveTab { get; set; }
        public RegisterViewModel Register { get; set; }
        public ResetPasswordModel ResetPassword { get; set; }
        public SignInModel SignIn { get; set; }
        public string ReturnUrl { get; set; }
        public FindLinkModel FindLinkModel { get; set; }
        public AccountInfo AccountInfo { get; set; }
    }
}