using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CUWebinars.Business.Models;
using CUWebinars.Web.Models;
using CUWebinars.Web.ViewModel;

namespace CUWebinars.Web.Models
{
    public class AdminDTO
    {
        public IList<OrderRow> OrderRows = new List<OrderRow>();
        public ShowWebinarsViewModel ShowWebinarsViewModel = new ShowWebinarsViewModel();
        public UserDetailsViewModel UserDetailsViewModel = new UserDetailsViewModel();
        public WpsViewModel WpsViewModel = new WpsViewModel();
        public CompPersSubscriptionsModel CompPersSubscriptionsModel = new CompPersSubscriptionsModel();
        public AffiliateSettingsViewModel AffiliateSettingsViewModel = new AffiliateSettingsViewModel();
        public IList<DiscountDTO> DiscountSubscriptionsModel = new List<DiscountDTO>();
        public InvoicesModel InvoicesModel = new InvoicesModel();

    }
}