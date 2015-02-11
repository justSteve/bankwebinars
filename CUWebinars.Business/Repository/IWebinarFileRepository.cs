using CUWebinars.Business.Models;
using System;
using System.Collections.Generic;

namespace CUWebinars.Business.Repository
{
    public interface IWebinarFileRepository : IDisposable
    {
        void AddRange(IEnumerable<WebinarFile> webinarFiles);
        void DeleteRange(IEnumerable<WebinarFile> webinarFiles);
        WebinarFile FindById(int id);
        void UpdateRange(IEnumerable<WebinarFile> webinarFiles);
    }
}
