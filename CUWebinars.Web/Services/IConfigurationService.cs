using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CUWebinars.Web.Services
{
    public interface IConfigurationService
    {
        string MailChimpId { get; }
    }
}
