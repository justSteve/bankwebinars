namespace CUWebinars.Web.Models
{
    public class JotFormOrderModel
    {
        public class Q4Name
        {
            public string first { get; set; }
            public string last { get; set; }
        }

        public class Q6PhoneNumber6
        {
            public string area { get; set; }
            public string phone { get; set; }
        }

        public class Q7Address
        {
            public string addr_line1 { get; set; }
            public string addr_line2 { get; set; }
            public string city { get; set; }
            public string state { get; set; }
            public string postal { get; set; }
            public string country { get; set; }
        }


        public string q12_webinarTitle { get; set; }
        public string q10_registrationType { get; set; }
        public Q4Name q4_name { get; set; }
        public string q8_institution { get; set; }
        public string q9_title { get; set; }
        public string q5_email5 { get; set; }
        public Q6PhoneNumber6 q6_phoneNumber6 { get; set; }
        public Q7Address q7_address { get; set; }
        public string q11_webinarid { get; set; }

    }
}