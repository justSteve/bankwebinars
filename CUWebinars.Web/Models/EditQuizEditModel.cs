
using System.Collections.Generic;
using CUWebinars.Business.Core;
using CUWebinars.Business.Models;

namespace CUWebinars.Web.Models
{
    public class EditQuizEditModel : AddQuizEditModel
    {
        public int QuizId { get; set; }

        public IEnumerable<int> DeletedQuestions { get; set; }
        public IEnumerable<EditedQuestion> EditedQuestions { get; set; }
        public IEnumerable<Question> NewQuestions { get; set; }
    }
}