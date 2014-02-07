namespace CUWebinars.Web.Notification
{
    public class Notification
    {
        public string ApplicationName { get; set; }
        public string CancelVerificationUrl { get; set; }
        public string ConfirmChangeEmailUrl { get; set; }
        public string ConfirmPasswordResetUrl { get; set; }
        public string Email { get; set; }
        public string EmailSignature { get; set; }
        public string LoginUrl { get; set; }
        public string VerificationKey { get; set; }

        public string Username { get; set; }
    }
}