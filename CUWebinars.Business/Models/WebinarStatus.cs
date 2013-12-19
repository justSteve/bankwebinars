using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CUWebinars.Business.Models
{
    public enum WebinarStatus
    {
        OnHold = 1,
        Scheduled = 2,
        Recorded = 3,
        Archived = 4,
        Deleted = 5
    }
}
