using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CUWebinars.Business.Models
{
    public partial class QuestionWithOption
    {
        public int Id { get; set; }
        public int idQuestion { get; set; }
        public int idOption { get; set; }
        public bool CorrectAnswer { get; set; }
        public string Letter { get; set; }

        public Question Question { get; set; }
        public Option Option { get; set; }
    }
}
