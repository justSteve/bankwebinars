using System;

namespace CUWebinars.Business.CQS
{
    public interface IPostCommitRegistrator
    {
        event Action Committed;
    }
}
