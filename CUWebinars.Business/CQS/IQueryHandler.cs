using System;

namespace CUWebinars.Business.CQS
{
    public interface IQueryHandler<in TQuery, out TResult> : IDisposable
        where TQuery : IQuery<TResult>
    {
        TResult Handle(TQuery query);
    }

}
