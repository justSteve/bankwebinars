using System;

namespace CUWebinars.Business.Models
{
    public partial class CitrixWebinar
    {
        public DateTime? DailyEndDate { get; set; }
        public int DailyFreq { get; set; } 
        public int DailyFreqCustom { get; set; }
        public string Description { get; set; }
        public int DescriptionMaxCharLimit { get; set; }
        public string EndHour { get; set; }
        public string EndMeridian { get; set; }
        public DateTime EndTime { get; set; }
        public int InterRecurIndex { get; set; }
        public string InternationalTollNumber { get; set; }
        public string InterRecurEndTimes { get; set; }
        public string InterRecurStartDates { get; set; }
        public string InterRecurStartTimes { get; set; }
        public string MonthlyDay { get; set; }
        public DateTime? MonthlyEnddate { get; set; }
        public int MonthlyFreq { get; set; }
        public string OrganizerAccessCode { get; set; }
        public string OrganizerPhoneNumber { get; set; }
        public string PanelistAccessCode { get; set; }
        public string PanelistPhoneNumber { get; set; }
        public string PrivateConfCallRadio { get; set; }
        public bool Pstn { get; set; }
        public bool PstnTF { get; set; }
        public string RecurrenceString { get; set; }
        public bool Recurs { get; set; }
        public string ReductiveSearch { get; set; }
        public bool RequirePassword { get; set; }
        public DateTime StartDate { get; set; } 
        public string StartHour { get; set; }
        public string StartMeridian { get; set; }
        public DateTime StartTime { get; set; }
        public string TemplateTitle { get; set; }
        public int TimeZoneKey { get; set; }
        public string TollConfCallCountryKeys { get; set; }
        public string TollFreeConfCallCountryKeys { get; set; }
        public bool Voip { get; set; }
        public string WebinarChoice{ get; set; }
        public string WebinarTitle { get; set; }
        public DateTime? WeeklyEnddate { get; set; }
        public int WeeklyFreqCustom { get; set; }
        public int WeeklyFreqDay { get; set; } 
    }
}
