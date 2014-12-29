using System;
using CUWebinars.Business.CQS;
using CUWebinars.Business.Models;
using CUWebinars.Tests.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ninject;

namespace CUWebinars.Business.Tests.UnitTests
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
        [TestCategory(TestCategories.Cqs)]
        public void QueryProcessorUsesCorrectHandler()
        {
            //  Arrange
            var query = new AdditionalLocationQuery();
            var queryProcessor = kernel.Get<IQueryProcessor>();

            //  Act
            var additionalLocation = queryProcessor.Process(query);

            //  Assert                    
            Assert.IsTrue(additionalLocation != null);
        }

        [TestMethod]
        [TestCategory(TestCategories.Cqs)]
        public void QueryProcessorThrowsExceptionWhenPassedNullReference()
        {
            //  Arrange
            AdditionalLocationQuery query = null;
            var queryProcessor = kernel.Get<IQueryProcessor>();

            //  Act
            // ReSharper disable once ExpressionIsAlwaysNull
            ExceptionAssert.Throws<NullReferenceException>(
                () => {var additionalLocation = queryProcessor.Process(query); }
                );
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
