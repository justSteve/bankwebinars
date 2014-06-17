using System;

namespace CUWebinars.Business.CQS
{
    public sealed class PostCommitRegistrator
        : IPostCommitRegistrator
    {
        public event Action Committed = () => { };

        public void ExecuteActions()
        {
            this.Committed();
        }

        public void Reset()
        {
            // Clears the list of actions.
            this.Committed = () => { };
        }
    }
}
