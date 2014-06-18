using CUWebinars.Business.CQS;
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
        [ExpectedException(typeof(RuntimeBinderException), "Cannot perform runtime binding on a null reference")]
        public void CommandProcessorThrowsExceptionWhenPassedNullReference()
        {
            //  Arrange
            SaveAdditionalLocationCommand saveAdditionalLocationCommand = null;
            var commandProcessor = kernel.Get<ICommandProcessor>();

            //  Act
            // ReSharper disable once ExpressionIsAlwaysNull
            commandProcessor.Execute(saveAdditionalLocationCommand);

            //  Assert                    
            Assert.Fail();
        }

        [TestMethod]
        [ExpectedException(typeof(ActivationException), "Error activating ICommandHandler{Object}")]
        public void CommandProcessorThrowsExceptionWhenPassedNonCommand()
        {
            //  Arrange
            object thisIsNotACommand = new object();
            var commandProcessor = kernel.Get<ICommandProcessor>();

            //  Act
            // ReSharper disable once ExpressionIsAlwaysNull
            commandProcessor.Execute(thisIsNotACommand);

            //  Assert                    
            Assert.Fail();
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
