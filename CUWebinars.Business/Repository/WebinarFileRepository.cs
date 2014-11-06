using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using CUWebinars.Business.Models;

namespace CUWebinars.Business.Repository
{
    public class WebinarFileRepository : TTSWebinarsRepository<TTSWebinarsContext, WebinarFile>, IWebinarFileRepository
    {
        public WebinarFileRepository()
        {
            
        }

        public WebinarFileRepository(TTSWebinarsContext context)
            : base(context)
        {

        }

        public void AddRange(IEnumerable<WebinarFile> webinarFiles)
        {

            items.AddRange(webinarFiles);
            db.SaveChanges();
        }

        public void DeleteRange(IEnumerable<WebinarFile> webinarFiles)
        {
            foreach (var webinarFile in webinarFiles)
            {
                CheckDisposed();

                var entry = db.Entry(webinarFile);

                if (entry.State == EntityState.Detached)
                {
                    items.Attach(webinarFile);
                    entry.State = EntityState.Deleted;
                }
            }
            db.SaveChanges();
        }

        public void UpdateRange(IEnumerable<WebinarFile> webinarFiles)
        {
            foreach (var webinarFile in webinarFiles)
            {
                CheckDisposed();

                var entry = db.Entry(webinarFile);

                if (entry.State == EntityState.Detached)
                {
                    items.Attach(webinarFile);
                    entry.State = EntityState.Modified;
                }
            }

            db.SaveChanges();
        }
    }
}