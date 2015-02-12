using System;

namespace CUWebinars.Web.Helpers
{
    [Serializable]
    public struct NotificationStorage
    {
        public int idOrder { get; set; }
        public SessionStartInfo SessionStartInfo { get; set; }
    }
}