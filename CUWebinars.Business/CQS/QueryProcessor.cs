using Ninject;

namespace CUWebinars.Business.CQS
{
    public class QueryProcessor : IQueryProcessor
    {
        private readonly IKernel _kernel;
        public QueryProcessor(IKernel kernel)
        {
            _kernel = kernel;
        }

        public TResult Process<TResult>(IQuery<TResult> query)
        {
            var queryHandlerType = typeof(IQueryHandler<,>).MakeGenericType(query.GetType(), typeof(TResult));

            dynamic queryHandler = _kernel.Get(queryHandlerType);

            return queryHandler.Handle((dynamic)query);

        }
    }
}
