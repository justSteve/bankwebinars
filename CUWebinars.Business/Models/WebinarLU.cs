using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CUWebinars.Business.Models
{
    public class WebinarLU
    {

        public int id { get; set; }
        public string name { get; set; }
        public int version { get; set; }
        public int source_id { get; set; }
        public DateTime created_at { get; set; }
        public bool sellable { get; set; }
        public bool cataloged { get; set; }
        public DateTime date_published { get; set; }
        public string keywords { get; set; }
        public object reference_code { get; set; }
        public bool manager_can_enroll { get; set; }
        public bool allow_users_rate_course { get; set; }
        public object due_days_after_enrollment { get; set; }
        public object due_date_after_enrollment { get; set; }
        public bool send_due_date_reminders { get; set; }
        public object due_date_reminder_days { get; set; }
        public object due_date_reminder_days_2 { get; set; }
        public int number_of_reviews { get; set; }
        public int number_of_stars { get; set; }
        public int minute_length { get; set; }
        public int num_enrolled { get; set; }
        public int num_not_started { get; set; }
        public int num_in_progress { get; set; }
        public int num_completed { get; set; }
        public int num_passed { get; set; }
        public int num_failed { get; set; }
        public int num_pending_review { get; set; }
        public int price { get; set; }
        public string published_status_id { get; set; }
        public string course_length_unit { get; set; }
        public string difficulty_level { get; set; }
        public int number_of_modules { get; set; }
        public string description_html { get; set; }
        public string description_text { get; set; }
        public string objectives_html { get; set; }
        public string objectives_text { get; set; }
        public string credits_to_be_awarded { get; set; }
    }

}
