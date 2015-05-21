
using System.Collections.Generic;
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

    public class EditedQuestion
    {
        public int QuestionId { get; set; }
        public int OriginalQuestionNumber { get; set; }
        public int NewQuestionNumber { get; set; }
        public IEnumerable<int> DeletedOptions { get; set; }
        public IEnumerable<EditableOption> EditedOptions { get; set; }
    }

    public class EditableOption
    {
        public int OptionId { get; set; }
        public string OriginalOptionLetter { get; set; }
        public string NewOptionLetter { get; set; }
        public string OriginalOptionText { get; set; }
        public string NewOptionText { get; set; }
        public bool OriginalCorrectStatus { get; set; }
        public bool NewCorrectStatus { get; set; }
    }
}