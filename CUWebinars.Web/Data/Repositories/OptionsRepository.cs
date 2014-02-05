using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using CUWebinars.Web.Data.Repositories.Interfaces;
using CUWebinars.Business.Models;

namespace CUWebinars.Web.Data.Repositories
{
    public class OptionRepository:IOptionsRepository

    {
        TTSWebinarsContext _ctx;

        public OptionRepository(TTSWebinarsContext ctx)
        {
            _ctx = ctx;
        }

        public OptionRepository()
        {
            throw new System.NotImplementedException();
        }

        public IQueryable<Option> GetOptionsByWebinar(int webinarId)
        {

            //var myOptions = _ctx.Webinars.Find(webinarId).OptionsGroup.Options.ToList();
            //return myOptions;
            return null;
        }


    }
}