using CUWebinars.Business.CQS;
using CUWebinars.Business.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ninject;
using System;

namespace CUWebinars.Business.Tests
{
    [TestClass]
    public class QueryProcessorTests
    {
        private IKernel kernel;

        public QueryProcessorTests()
        {
            kernel = new StandardKernel();
            kernel.Bind<ICommandProcessor>().To<CommandProcessor>().InTransientScope();
            kernel.Bind<IQueryProcessor>().To<QueryProcessor>().InTransientScope();

            BusinessTestHelper.AutoRegisterType(typeof(IQueryHandler<,>), kernel); // Register IQueryHandler 

        }

        [TestMethod]
        public void QueryProcessorUsesCorrectHandler()
        {
            //  Arrange
            var query = new AdditionalLocationQuery();
            var queryProcessor = kernel.Get<IQueryProcessor>();

            //  Act
            var al = queryProcessor.Process(query);

            //  Assert                    
            Assert.IsTrue(al != null);
        }

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public void QueryProcessorThrowsExceptionWhenPassedNullReference()
        {
            //  Arrange
            AdditionalLocationQuery query = null;
            var queryProcessor = kernel.Get<IQueryProcessor>();

            //  Act
            // ReSharper disable once ExpressionIsAlwaysNull
            var al = queryProcessor.Process(query);

            //  Assert                    
            Assert.Fail();
        }

#region Nested helper classes
		
        public class AdditionalLocationQuery : IQuery<AdditionalLocation>
        {
            public int Id { get; set; }
        }

        public class AddLocationQueriesFake : IQueryHandler<AdditionalLocationQuery, AdditionalLocation>
        {
            public AdditionalLocation Handle(AdditionalLocationQuery query)
            {
                return new AdditionalLocation();
            }

            public void Dispose()
            {
                throw new NotImplementedException();
            }
        }

#endregion    
    }
}
