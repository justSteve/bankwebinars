using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using CUWebinars.Web.Models;

namespace CUWebinars.Web.ViewModel
{
    public class EditUserViewModel
    {

        [UIHint("EditUser")]
        public EditUserModel EditFields { get; set; }
    }
}