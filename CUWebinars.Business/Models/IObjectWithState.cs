
using System.Collections.Generic;

namespace CUWebinars.Business.Models
{
    public interface IObjectWithState
    {
        State EntityState { get; set; }
        Dictionary<string, object> OriginalValues { get; set; }
    }


    public enum State
    {
        Added,
        Unchanged,
        Deleted
    }
}
