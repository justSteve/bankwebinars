namespace CUWebinars.Web.Models
{
    public class ExpressCheckoutModel
    {
        public string slug { get; set; }
        public string q12_webinarTitle { get; set; }
        public string q10_registrationType { get; set; }
        public Q4Name q4_name { get; set; }
        public string q9_title { get; set; }
        public string q8_institution { get; set; }
        public string q5_email5 { get; set; }
        public Q6PhoneNumber6 q6_phoneNumber6 { get; set; }
        public Q14Address14 q14_address14 { get; set; }
        public int q11_webinarid { get; set; }
        public int q15_affiliateid15 { get; set; }
        public string event_id { get; set; }
    }
}