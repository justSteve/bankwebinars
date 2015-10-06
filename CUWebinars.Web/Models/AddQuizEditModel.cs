using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using CUWebinars.Business.Models;

namespace CUWebinars.Web.Models
{

    public class AddQuizEditModel
    {
        public IEnumerable<Question> Questions { get; set; }
        [Display(Name = "Select a Webinar")]
        public int SelectedWebinar { get; set; }
    }
}