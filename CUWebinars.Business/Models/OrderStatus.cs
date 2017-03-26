
namespace CUWebinars.Business.Models
{
    public enum OrderStatus
    {
        Error = 0,  //cancelled
        InProcess = 1,
        Submitted = 2,
        Billed = 3,
        Paid = 4,
        Abandoned = 5, //cancelled
        Canceled = 6, //cancelled
        AwaitingVerification = 7,
        OutstandingBalance = 8,
        Unknown = 255 //cancelled
    }
}
