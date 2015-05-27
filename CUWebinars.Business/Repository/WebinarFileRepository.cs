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

                var file = items.SingleOrDefault(wf => wf.idWebinarFile == webinarFile.idWebinarFile);

                if (ReferenceEquals(null, file))
                {
                    var entry = db.Entry(webinarFile);

                    if (entry.State == EntityState.Detached)
                    {
                        items.Attach(webinarFile);
                        entry.State = EntityState.Deleted;
                    }
                }
                else
                {
                    items.Remove(file);
                }

            }
            db.SaveChanges();
        }

        public void UpdateRange(IEnumerable<WebinarFile> webinarFiles)
        {
            foreach (var webinarFile in webinarFiles)
            {
                CheckDisposed();

                var file = items.SingleOrDefault(wf => wf.idWebinarFile == webinarFile.idWebinarFile);

                if (ReferenceEquals(null, file))
                {
                    var entry = db.Entry(webinarFile);

                    if (entry.State == EntityState.Detached)
                    {
                        items.Attach(webinarFile);
                        entry.State = EntityState.Modified;
                    }
                }
                else
                {
                    file.fileDesc = webinarFile.fileDesc;
                    file.fileLocation= webinarFile.fileLocation;
                    db.Entry(file).State = EntityState.Modified;
                }
            }

            db.SaveChanges();
        }
    }
}