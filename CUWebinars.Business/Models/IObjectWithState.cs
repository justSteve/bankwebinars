
namespace CUWebinars.Business.Models
{
    public interface IObjectWithState
    {
        State State { get; set; }
    }


    public enum State
    {
        Added,
        Unchanged,
        Deleted
    }
}
