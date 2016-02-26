namespace CUWebinars.Business.Models
{
    public class Royalty
    {
        public Affiliate Affiliate { get; set; }
        public Order Order { get; set; }
        public decimal Amount { get; set; }
    }
}