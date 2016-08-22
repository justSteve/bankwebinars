namespace CUWebinars.Web.Models
{
    public class CitrixAudioModel
    {
        public ConfCallNumbers ConfCallNumbers { get; set; }

        public string Type { get; set; }
    }

    public class ConfCallNumbers
    {
        public string US { get; set; }
        public AccessCodes AccessCodes { get; set; }
        public string TollFree { get; set; }
    }

    public class AccessCodes
    {
        public string Organizer { get; set; }
        public string Panelist { get; set; }
        public string Attendee { get; set; }
    }
}