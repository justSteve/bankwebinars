using System;
using CUWebinars.Business.Models;
using System.Collections.Generic;

namespace CUWebinars.Business.Repository
{
    public interface IDiscountRepository : IDisposable
    {
        Discount FindDiscount(int idDiscount);

        void SaveChanges(Discount discount);
    }
}
