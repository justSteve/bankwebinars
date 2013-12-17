namespace CUWebinars.Business.Models
{
    public partial class WebinarTopicXref
    {
        public int idWebinarTopicXref { get; set; }
        public int idWebinar { get; set; }
        public int idTopic { get; set; }
        public virtual Topic Topic { get; set; }
        public virtual Webinar Webinar { get; set; }
    }
}
