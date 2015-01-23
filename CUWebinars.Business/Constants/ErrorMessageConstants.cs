
namespace CUWebinars.Business.Constants
{
    public sealed class ErrorMessageConstants
    {
        public const string ExistingNonCancelledOrderMessage =
            "The WebUser already has at least 1 order which is in a non-cancelled state";
        public const string IdLessThan1Message = "The WebUserId was less than 1.";
    }
}
