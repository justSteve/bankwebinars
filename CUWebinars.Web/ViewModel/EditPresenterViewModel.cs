using System.ComponentModel.DataAnnotations;
using CUWebinars.Web.Models;

namespace CUWebinars.Web.ViewModel
{
    public class  EditPresenterViewModel
    {

        [UIHint("EditPresenter")]
        public EditPresenterModel EditFields { get; set; }
    }
}