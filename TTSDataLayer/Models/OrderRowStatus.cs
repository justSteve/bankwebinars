namespace CUWebinars.Business.Models
{
    public enum OrderRowStatus
    {
        Error = 0,
        InProcess = 1,
        Submitted = 2,
        Billed = 3,
        Paid = 4,
        Abandoned = 5,
        Canceled = 6,
        AwaitingVerification = 7,
        PossibleDupeUsers = 8,
        PendingAuth = 9,
        Unknown = 255

    }
}
