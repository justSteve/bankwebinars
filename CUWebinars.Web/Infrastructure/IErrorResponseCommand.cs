namespace CUWebinars.Web.Infrastructure
{
    internal interface IErrorResponseCommand
    {
        void Execute(ErrorResponse errorResponse);
    }
}