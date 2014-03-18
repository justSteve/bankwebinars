namespace CUWebinars.Business.Models
{
    public partial class AdditionalLocation
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public int idOrderRowOption { get; set; }
        public virtual OrderRowOption OrderRowOption { get; set; }
    }
}
