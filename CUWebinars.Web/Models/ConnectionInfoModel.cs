using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using CUWebinars.Business.Models;

namespace CUWebinars.Web.Models
{
    public class ConnectionInfoModel
    {
        public ICollection<WebinarFile> WebinarFiles { get; set; }
    }
}