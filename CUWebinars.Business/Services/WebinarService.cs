using CUWebinars.Business.Models;
using System.Net;

namespace CUWebinars.Business.Services
{
    public class WebinarService
    {
        public string CreateWebinar()
        {
            string HtmlResult = string.Empty;
            string URI = "https://global.gotowebinar.com/schedule.tmpl";
            
            var citrixWebinar = new CitrixWebinar(); // this will actually be retrieved from database. We can also manipulate it here before sending

            string payLoad = string.Format(@"
                Template={0}&Form={1}&Action={2}&tollConfCallCountryKeys={3}&tollFreeConfCallCountryKeys={4}&RecurrenceString={5}&
                StartTime={6}&EndTime={7}&AllowFreeConfCall={8}&AllowPrivateConfCall={9}&WebinarChoice={10}&WebinarTitle={11}&Description={12}&
                Description_MaxCharLimit={13}&StartDate_Cal_0_hidden={14}&StartDate_Cal_0={15}&StartHour_0={16}&StartAMPM_0={17}&EndHour_0={18}&
                EndAMPM_0={19}&TimeZoneKey={20}&Recurs={21}&daily_freq={22}&daily_freq_custom={23}&daily_freq={24}&weekly_freq_custom={25}&
                monthly_freq_custom={26}&monthly_freq={27}&monthly_day={28}&daily_enddate_hidden={29}&daily_enddate={30}&weekly_enddate_hidden={31}&
                weekly_enddate={32}&monthly_enddate_hidden={33}&monthly_enddate={34}&InterRecurStartDates={35}&InterRecurStartTimes={36}&
                InterRecurEndTimes={37}&inter_recur_index={38}&StartDate_Cal_1_hidden={39}&StartDate_Cal_1={40}&StartHour_1={41}&StartAMPM_1={42}&
                EndHour_1={43}&EndAMPM_1={44}&attendee_type_0={45}&attendee_type_1={46}&reductiveSearch={47}&voip={48}&pstn={49}&pstnTF={50}&
                privateConfCallRadio={51}&organizerPhoneNumber={52}&panelistPhoneNumber={53}&attendeePhoneNumber={54}&organizerAccessCode={55}&
                panelistAccessCode={56}&attendeeAccessCode={57}&internationalTollNumber={58}&CoOrganizers_1={59}&CoOrganizers_2={60}&
                Panelist1Name_Full={61}&Panelist1Email={62}&RequirePassword={63}", citrixWebinar.Template,
                citrixWebinar.Form, 
                citrixWebinar.Action,
                citrixWebinar.TollConfCallCountryKeys, 
                citrixWebinar.TollFreeConfCallCountryKeys,
                citrixWebinar.RecurrenceString, 
                citrixWebinar.StartTime, 
                citrixWebinar.EndTime,
                citrixWebinar.AllowFreeConfCall, 
                citrixWebinar.AllowPrivateConfCall,
                citrixWebinar.WebinarChoice, 
                citrixWebinar.WebinarTitle, 
                citrixWebinar.Description,
                citrixWebinar.Description_MaxCharLimit, 
                citrixWebinar.StartDate_Cal_0_hidden,
                citrixWebinar.StartDate_Cal_0, 
                citrixWebinar.StartHour_0, 
                citrixWebinar.StartAMPM_0,
                citrixWebinar.EndHour_0, 
                citrixWebinar.EndAMPM_0, 
                citrixWebinar.TimeZoneKey,
                citrixWebinar.Recurs, 
                citrixWebinar.Daily_freq, 
                citrixWebinar.Daily_freq_custom,
                citrixWebinar.Daily_freq, 
                citrixWebinar.Weekly_freq_custom, 
                citrixWebinar.Monthly_freq_custom,
                citrixWebinar.Monthly_freq, 
                citrixWebinar.Monthly_day, 
                citrixWebinar.Daily_enddate_hidden,
                citrixWebinar.Daily_enddate, 
                citrixWebinar.Weekly_enddate_hidden, 
                citrixWebinar.Weekly_enddate,
                citrixWebinar.Monthly_enddate_hidden, 
                citrixWebinar.Monthly_enddate, 
                citrixWebinar.InterRecurStartDates,
                citrixWebinar.InterRecurStartTimes, 
                citrixWebinar.InterRecurEndTimes, 
                citrixWebinar.Inter_recur_index,
                citrixWebinar.StartDate_Cal_1_hidden, 
                citrixWebinar.StartDate_Cal_1, 
                citrixWebinar.StartHour_1,
                citrixWebinar.StartAMPM_1, 
                citrixWebinar.EndHour_1, 
                citrixWebinar.EndAMPM_1,
                citrixWebinar.Attendee_type_0,
                citrixWebinar.Attendee_type_1, 
                citrixWebinar.ReductiveSearch, 
                citrixWebinar.Voip, 
                citrixWebinar.Pstn,
                citrixWebinar.PstnTF, 
                citrixWebinar.PrivateConfCallRadio, 
                citrixWebinar.OrganizerPhoneNumber,
                citrixWebinar.PanelistPhoneNumber,
                citrixWebinar.AttendeePhoneNumber, 
                citrixWebinar.OrganizerAccessCode, 
                citrixWebinar.PanelistAccessCode,
                citrixWebinar.AttendeeAccessCode, 
                citrixWebinar.InternationalTollNumber, 
                citrixWebinar.CoOrganizers_1,
                citrixWebinar.CoOrganizers_2, 
                citrixWebinar.Panelist1Name_Full, 
                citrixWebinar.Panelist1Email,
                citrixWebinar.RequirePassword
                );

            //  do some kind of trim to get rid of all white space in the readable version of the payload above.

            using (var webClient = new WebClient())
            {
                webClient.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded"; // should be a constant

                HtmlResult = webClient.UploadString(URI, payLoad);
            }

            return HtmlResult;
        }
    }
}
