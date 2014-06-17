
namespace CUWebinars.Business.CQS.Commands
{
    public class VerifyAccountCommand
    {
        public string TempPassword { get; set; }
        public string VerificationKey { get; set; }
    }
}
