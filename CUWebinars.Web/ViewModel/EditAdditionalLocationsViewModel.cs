namespace CUWebinars.Web.ViewModel
{
    public class EditAdditionalLocationsViewModel
    {
        public decimal CostPerAdditionalLocation { get; set; }
        public int idOrderRow { get; set; }
        public int idUser { get; set; }
        public int WebinarId { get; set; }
        public int NumberOfAdditionalLocations { get; set; }
        public decimal TotalCostOfOptions { get; set; }
        public int idOrder { get; set; }
    }
}