using CUWebinars.Business.CQS;
using CUWebinars.Tests.Common;
using Microsoft.CSharp.RuntimeBinder;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ninject;
using System;

namespace CUWebinars.Business.Tests
{
    [TestClass]
    public class CommandProcessorTests
    {
        private IKernel kernel;
        public CommandProcessorTests()
        {
            kernel = new StandardKernel();

            kernel.Bind<ICommandProcessor>().To<CommandProcessor>().InTransientScope();
            BusinessTestHelper.AutoRegisterType(typeof(ICommandHandler<>), kernel); // Register IQueryHandler 
        }

        [TestMethod]
        public void CommandProcessorUsesCorrectHandler()
        {
            //  Arrange
            var command = new SaveAdditionalLocationCommand();
            var commandProcessor = kernel.Get<ICommandProcessor>();

            //  Act
            commandProcessor.Execute(command);

            //  Assert                    
            Assert.IsTrue(SaveAdditionalLocationCommandHandlerFake.HandleWasCalled);
        }

        [TestMethod]
        public void CommandProcessorThrowsExceptionWhenPassedNullReference()
        {
            //  Arrange
            SaveAdditionalLocationCommand saveAdditionalLocationCommand = null;
            var commandProcessor = kernel.Get<ICommandProcessor>();

            //  Act & Assert
            // ReSharper disable once ExpressionIsAlwaysNull

            ExceptionAssert.Throws<RuntimeBinderException>(
                () => commandProcessor.Execute(saveAdditionalLocationCommand),
                "Cannot perform runtime binding on a null reference");
        }

        [TestMethod]
        public void CommandProcessorThrowsExceptionWhenPassedNonCommand()
        {
            //  Arrange
            object thisIsNotACommand = new object();
            var commandProcessor = kernel.Get<ICommandProcessor>();

            //  Act & Assert
            // ReSharper disable once ExpressionIsAlwaysNull
            ExceptionAssert.Throws<ActivationException>(
                () => commandProcessor.Execute(thisIsNotACommand),
                "Error activating ICommandHandler{Object}");
        }
        
        #region Nested helper classes

        public class SaveAdditionalLocationCommandHandlerFake : ICommandHandler<SaveAdditionalLocationCommand>
        {
            public static bool HandleWasCalled { get; set; }

            public void Handle(SaveAdditionalLocationCommand command)
            {
                HandleWasCalled = true;
            }

            public void Dispose()
            {
                throw new NotImplementedException();
            }
        }

        public class SaveAdditionalLocationCommand
        {
        }
        
        #endregion
    }
}
