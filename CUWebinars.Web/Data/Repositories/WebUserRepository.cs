using System;
using System.Collections.Generic;
using System.Linq;
using CUWebinars.Web.Data.Repositories.Interfaces;
using TTSDataLayer.Models;

namespace CUWebinars.Web.Data.Repositories
{
    public class WebUserRepository : IWebUserRepository
    {
        public TTSWebinarsContext _ctx { get; set; }

        public WebUserRepository(TTSWebinarsContext ctx)
        {
            _ctx = ctx;
        }

        public IList<WebUser> GetWebUsers()
        {
            
            var query = from w in _ctx.WebUsers
                        where w.idUser == 19
                        select w;

            var thisList = query;
            return thisList.Take(5).ToList();
        }

        public int FindInstitution(string zip, string institutionName)
        {
            //int myInst = _ctx.Institutions
            //    .Where(i => i.InstitutionName == institutionName && i.ZIP == Zip
            //        .Select (i => i.Id));
            return 0;
        }

        public IQueryable<WebUser> GetUsers()
        {
            throw new NotImplementedException();
        }
    }
}