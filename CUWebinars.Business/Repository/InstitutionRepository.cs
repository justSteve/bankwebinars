using System;
using CUWebinars.Business.Models;
using Ninject.Extensions.Logging;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using CUWebinars.Business.Core;

namespace CUWebinars.Business.Repository
{
    public class InstitutionRepository : TTSWebinarsRepository<TTSWebinarsContext, Institution>, IInstitutionRepository
    {
        public InstitutionRepository(TTSWebinarsContext ctx)
            : base(ctx)
        {

        }

        public int FindFirst()
        {
            return items.First().idInstitution;
        }

        public Institution GetByDomain(string domain)
        {
            return items.FirstOrDefault(i => i.domainName.ToLower().Equals(domain.ToLower()));
        }

        public IEnumerable<Institution> GetByNameAndZipCode(string name, string zip)
        {
            return items.Where(i => i.InstitutionName == name && i.Zip == zip);
        }

        public IEnumerable<Institution> GetInstitutionsByName(string name)
        {
            return items.Where(i => i.InstitutionName.ToLower().Contains(name.ToLower()));
        }

        public Institution GetById(int idInstitution)
        {
            return items.Single(i => i.idInstitution== idInstitution);
        }

        public void Update(Institution institution)
        {
            CheckDisposed();

            var entry = db.Entry(institution);
            if (entry.State == EntityState.Detached)
            {
                items.Attach(institution);
                entry.State = EntityState.Modified;
            }
            db.SaveChanges();
        }

    }
}
