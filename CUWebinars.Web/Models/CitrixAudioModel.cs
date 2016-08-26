namespace CUWebinars.Web.Models
{
    public class CitrixAudioModel
    {
        public ConfCallNumbers ConfCallNumbers { get; set; }

        public string Type { get; set; }
    }

    public class ConfCallNumbers
    {
        public US US { get; set; } // seems super-restrictive to need to make a class for each country we need (or am I missing something?).  May not be worth fighting, but asking Citrix about restructering it would be interesting.
    }

    public class US
    {
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