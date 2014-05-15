using CUWebinars.Business.Models;
using System.Net;

namespace CUWebinars.Business.Services
{
    public class WebinarService
    {
//        public string CreateWebinar()
//        {
//            string HtmlResult = string.Empty;
//            string URI = "https://global.gotowebinar.com/schedule.tmpl";
            
//            var GTWebinar = new GTWebinar(); // this will actually be retrieved from database. We can also manipulate it here before sending

//            string payLoad = string.Format(@"
//                Template={0}&Form={1}&Action={2}&tollConfCallCountryKeys={3}&tollFreeConfCallCountryKeys={4}&RecurrenceString={5}&
//                StartTime={6}&EndTime={7}&AllowFreeConfCall={8}&AllowPrivateConfCall={9}&WebinarChoice={10}&WebinarTitle={11}&Description={12}&
//                Description_MaxCharLimit={13}&StartDate_Cal_0_hidden={14}&StartDate_Cal_0={15}&StartHour_0={16}&StartAMPM_0={17}&EndHour_0={18}&
//                EndAMPM_0={19}&TimeZoneKey={20}&Recurs={21}&daily_freq={22}&daily_freq_custom={23}&daily_freq={24}&weekly_freq_custom={25}&
//                monthly_freq_custom={26}&monthly_freq={27}&monthly_day={28}&daily_enddate_hidden={29}&daily_enddate={30}&weekly_enddate_hidden={31}&
//                weekly_enddate={32}&monthly_enddate_hidden={33}&monthly_enddate={34}&InterRecurStartDates={35}&InterRecurStartTimes={36}&
//                InterRecurEndTimes={37}&inter_recur_index={38}&StartDate_Cal_1_hidden={39}&StartDate_Cal_1={40}&StartHour_1={41}&StartAMPM_1={42}&
//                EndHour_1={43}&EndAMPM_1={44}&attendee_type_0={45}&attendee_type_1={46}&reductiveSearch={47}&voip={48}&pstn={49}&pstnTF={50}&
//                privateConfCallRadio={51}&organizerPhoneNumber={52}&panelistPhoneNumber={53}&attendeePhoneNumber={54}&organizerAccessCode={55}&
//                panelistAccessCode={56}&attendeeAccessCode={57}&internationalTollNumber={58}&CoOrganizers_1={59}&CoOrganizers_2={60}&
//                Panelist1Name_Full={61}&Panelist1Email={62}&RequirePassword={63}", GTWebinar.Template,
//                GTWebinar.Form, 
//                GTWebinar.Action,
//                GTWebinar.TollConfCallCountryKeys, 
//                GTWebinar.TollFreeConfCallCountryKeys,
//                GTWebinar.RecurrenceString, 
//                GTWebinar.StartTime, 
//                GTWebinar.EndTime,
//                GTWebinar.AllowFreeConfCall, 
//                GTWebinar.AllowPrivateConfCall,
//                GTWebinar.WebinarChoice, 
//                GTWebinar.WebinarTitle, 
//                GTWebinar.Description,
//                GTWebinar.Description_MaxCharLimit, 
//                GTWebinar.StartDate_Cal_0_hidden,
//                GTWebinar.StartDate_Cal_0, 
//                GTWebinar.StartHour_0, 
//                GTWebinar.StartAMPM_0,
//                GTWebinar.EndHour_0, 
//                GTWebinar.EndAMPM_0, 
//                GTWebinar.TimeZoneKey,
//                GTWebinar.Recurs, 
//                GTWebinar.Daily_freq, 
//                GTWebinar.Daily_freq_custom,
//                GTWebinar.Daily_freq, 
//                GTWebinar.Weekly_freq_custom, 
//                GTWebinar.Monthly_freq_custom,
//                GTWebinar.Monthly_freq, 
//                GTWebinar.Monthly_day, 
//                GTWebinar.Daily_enddate_hidden,
//                GTWebinar.Daily_enddate, 
//                GTWebinar.Weekly_enddate_hidden, 
//                GTWebinar.Weekly_enddate,
//                GTWebinar.Monthly_enddate_hidden, 
//                GTWebinar.Monthly_enddate, 
//                GTWebinar.InterRecurStartDates,
//                GTWebinar.InterRecurStartTimes, 
//                GTWebinar.InterRecurEndTimes, 
//                GTWebinar.Inter_recur_index,
//                GTWebinar.StartDate_Cal_1_hidden, 
//                GTWebinar.StartDate_Cal_1, 
//                GTWebinar.StartHour_1,
//                GTWebinar.StartAMPM_1, 
//                GTWebinar.EndHour_1, 
//                GTWebinar.EndAMPM_1,
//                GTWebinar.Attendee_type_0,
//                GTWebinar.Attendee_type_1, 
//                GTWebinar.ReductiveSearch, 
//                GTWebinar.Voip, 
//                GTWebinar.Pstn,
//                GTWebinar.PstnTF, 
//                GTWebinar.PrivateConfCallRadio, 
//                GTWebinar.OrganizerPhoneNumber,
//                GTWebinar.PanelistPhoneNumber,
//                GTWebinar.AttendeePhoneNumber, 
//                GTWebinar.OrganizerAccessCode, 
//                GTWebinar.PanelistAccessCode,
//                GTWebinar.AttendeeAccessCode, 
//                GTWebinar.InternationalTollNumber, 
//                GTWebinar.CoOrganizers_1,
//                GTWebinar.CoOrganizers_2, 
//                GTWebinar.Panelist1Name_Full, 
//                GTWebinar.Panelist1Email,
//                GTWebinar.RequirePassword
//                );

//            //  do some kind of trim to get rid of all white space in the readable version of the payload above.

//            using (var webClient = new WebClient())
//            {
//                webClient.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded"; // should be a constant

//                HtmlResult = webClient.UploadString(URI, payLoad);
//            }

//            return HtmlResult;
//        }
    }
}
