using System.Web;
using System.Web.SessionState;

namespace CUWebinars.Web.Services
{
    public class StateService : IStateService
    {
        private HttpSessionState SessionState
        {
            get
            {
                return HttpContext.Current.Session;
            }
        }

        public void SetValue<T>(string key, T value)
        {
            if (!ReferenceEquals(this.SessionState, null))
            this.SessionState.Add(key, value);
        }

        public void ClearValue(string key)
        {
            this.SessionState.Remove(key);
        }
            
        public T GetValue<T>(string key)
        {
            return (T)this.SessionState[key];
        }

        public bool HasValue(string key)
        {
            if (this.SessionState == null)
                return false;
            foreach (string k in this.SessionState.Keys)
            {
                if (key == k)
                    return true;
            }
            return false;
        }
    }
}