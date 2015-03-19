using System;
using System.Linq.Expressions;
using CUWebinars.Business.Models;
using System.Collections.Generic;

namespace CUWebinars.Business.Repository
{
    public interface IInstitutionRepository : IDisposable
    {
        void Add(Institution institution);
        int FindFirst();
        Institution GetByDomain(string domain);
        void Remove(Institution institution);
        IEnumerable<Institution> GetAll();
        IEnumerable<Institution> GetAllIncluding(params Expression<Func<Institution, object>>[] includeProperties);
        IEnumerable<Institution> GetByNameAndZipCode(string name, string zip);
        IEnumerable<Institution> GetInstitutionsByName(string name);
    }
}
