using System;

namespace CUWebinars.Business.CQS
{
    public interface ICommandHandler<in TCommand> : IDisposable
    {
        void Handle(TCommand command);
    }
}