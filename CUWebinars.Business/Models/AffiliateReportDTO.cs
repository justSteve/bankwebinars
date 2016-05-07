using System;
using System.Collections.Generic;
using CUWebinars.Business.Models;

namespace CUWebinars.Web.Models
{
	public class AffiliateReportDTO
	{
		public AffiliateReportDTO()
		{
			Orders = new List<Order>();
		}

		public int WebinarId { get; set; }
		public AffiliateInvoiceDTO Invoice { get; set; }
		

		public Affiliate Affiliate { get; set; }
		public IList<Order> Orders { get; set; }
		public decimal TotalRevenues { get; set; }
		public decimal TotalCommissions { get; set; }
		public decimal DueOnBilledTotal { get; set; }
		public decimal RoyaltyOnPaidTotal { get; set; }
		public decimal NetDueTTSTotal { get; set; }

	}
}