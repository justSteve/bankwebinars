namespace CUWebinars.Business.Models
{
    public partial class AdditionalEmails
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        //public int idAdditionalLocation { get; set; }
        public virtual AdditionalLocations AdditionalLocations { get; set; }
    }
}
