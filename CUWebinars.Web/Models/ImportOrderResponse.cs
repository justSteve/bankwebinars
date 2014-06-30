namespace CUWebinars.Web.Models
{
    public class ImportOrderResponse
    {
        public string Status { get; set; }
        public int rowIndex { get; set; }

        public bool IsError { get; set; }
    }
}