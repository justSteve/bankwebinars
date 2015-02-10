namespace CUWebinars.Web.ViewModel
{
    public class AffPromoMsgViewModel
    {
        public int AffID { get; set; }
        public string MsgBody { get; set; }

        public AffPromoMsgViewModel(int id, string msg)
        {
            AffID = id;
            MsgBody = msg;


        }
    }
}