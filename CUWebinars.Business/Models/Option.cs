
using System.Collections.Generic;

namespace CUWebinars.Business.Models
{
    public partial class Option
    {
        public Option()
        {
            this.QuestionWithOptions = new List<QuestionWithOption>();
        }

        public int Id { get; set; }
        public string Text { get; set; }
        public virtual ICollection<QuestionWithOption> QuestionWithOptions { get; set; }
    }
}
