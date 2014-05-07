using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CUWebinars.Business.Models
{
    public partial class CitrixWebinar
    {
        public string Action { get; set; }
        public string AllowFreeConfCall { get; set; }
        public string AllowPrivateConfCall { get; set; }
        public string AttendeeAccessCode { get; set; }
        public string AttendeePhoneNumber { get; set; }
        public string Attendee_type_0 { get; set; }
        public string Attendee_type_1 { get; set; }
        public string CoOrganizers_1 { get; set; }
        public string CoOrganizers_2 { get; set; }
        public DateTime Daily_enddate { get; set; }
        public DateTime Daily_enddate_hidden { get; set; }
        public int Daily_freq { get; set; } //  gets repeated
        public int Daily_freq_custom { get; set; }
        public string Description { get; set; }
        public int Description_MaxCharLimit { get; set; }
        public DateTime EndAMPM_0 { get; set; } // ???????
        public DateTime EndAMPM_1 { get; set; } // ???????
        public DateTime EndHour_0 { get; set; } // ???????
        public DateTime EndHour_1 { get; set; } // ???????
        public DateTime EndTime { get; set; }
        public string Form { get; set; }
        public int Inter_recur_index { get; set; }
        public string InternationalTollNumber { get; set; }
        public string InterRecurEndTimes { get; set; }
        public string InterRecurStartDates { get; set; }
        public string InterRecurStartTimes { get; set; }
        public string Monthly_day { get; set; }
        public DateTime Monthly_enddate { get; set; }
        public DateTime Monthly_enddate_hidden { get; set; }
        public int Monthly_freq { get; set; } // repeated
        public int Monthly_freq_custom { get; set; }
        public string OrganizerAccessCode { get; set; }
        public string OrganizerPhoneNumber { get; set; }
        public string PanelistAccessCode { get; set; }
        public string Panelist1Email { get; set; }
        public string Panelist1Name_Full { get; set; }
        public string PanelistPhoneNumber { get; set; }
        public string PrivateConfCallRadio { get; set; }
        public bool Pstn { get; set; }
        public bool PstnTF { get; set; }
        public string RecurrenceString { get; set; }
        public bool Recurs { get; set; }
        public string ReductiveSearch { get; set; }
        public bool RequirePassword { get; set; }
        public int ScheduleAWebinar { get; set; }
        public DateTime StartAMPM_0 { get; set; } // ???????
        public DateTime StartAMPM_1 { get; set; } // ???????
        public DateTime StartDate_Cal_0 { get; set; } // ???????
        public DateTime StartDate_Cal_0_hidden { get; set; } // ???????
        public DateTime StartDate_Cal_1 { get; set; } // ???????
        public DateTime StartDate_Cal_1_hidden { get; set; } // ???????
        public DateTime StartDate_1 { get; set; } // ???????
        public DateTime StartHour_0 { get; set; } // ???????
        public DateTime StartHour_1 { get; set; } // ???????
        public DateTime StartTime { get; set; }
        public string Template { get; set; }
        public int TimeZoneKey { get; set; }
        public string TollConfCallCountryKeys { get; set; }
        public string TollFreeConfCallCountryKeys { get; set; }
        public bool Voip { get; set; }
        public string WebinarChoice{ get; set; }
        public string WebinarTitle { get; set; }
        public DateTime Weekly_enddate { get; set; }
        public DateTime Weekly_enddate_hidden { get; set; }
        public int Weekly_freq_custom { get; set; }
        public int Weekly_freq_day { get; set; } // repeats 6 times
    }
}
