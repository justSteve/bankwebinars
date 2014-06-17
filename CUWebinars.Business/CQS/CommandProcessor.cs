using Ninject;
using System;

namespace CUWebinars.Business.CQS
{
    public class CommandProcessor : ICommandProcessor
    {
        private readonly IKernel _kernel;

        public CommandProcessor(IKernel kernel)
        {
            _kernel = kernel;
        }

        public void Execute(dynamic command)
        {
            Type commandHandlerType = typeof(ICommandHandler<>).MakeGenericType(command.GetType());
            dynamic commandHandler = _kernel.Get(commandHandlerType);
            commandHandler.Handle(command);
        }
    }
}
