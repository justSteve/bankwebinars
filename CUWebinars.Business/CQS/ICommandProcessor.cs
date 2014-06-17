
namespace CUWebinars.Business.CQS
{
    public interface ICommandProcessor
    {
        void Execute(dynamic command);
    }

}
