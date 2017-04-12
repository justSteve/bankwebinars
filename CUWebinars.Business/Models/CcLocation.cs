namespace CUWebinars.Business.Models
{
    public class CcLocation
    {
        public int Id { get; set; }
        public int idOrderRow { get; set; }
        //Description to use in Cart
        public string DescriptionPromo { get; set; }
        //Description to use after purchased
        public string DescriptionConfirm { get; set; }
        
        public virtual OrderRow OrderRow { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
    }
}