using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CUWebinars.Business.Models;
using CUWebinars.Web.ViewModel;

namespace CUWebinars.Web.Models
{
    public class AdminDTO
    {
        public IList<OrderRow> OrderRows = new List<OrderRow>();
        public ShowWebinarsViewModel ShowWebinarsViewModel = new ShowWebinarsViewModel();
        public UserDetailsViewModel  UserDetailsViewModel= new UserDetailsViewModel();
        public AffiliateSettingsViewModel AffiliateSettingsViewModel = new AffiliateSettingsViewModel();
        public SubscriptionsViewModel SubscriptionsViewModel = new SubscriptionsViewModel();
    }
}