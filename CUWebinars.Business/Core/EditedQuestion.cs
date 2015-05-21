using System.Collections.Generic;

namespace CUWebinars.Business.Core
{
    public class EditedQuestion
    {
        public int QuestionId { get; set; }
        public int OriginalQuestionNumber { get; set; }
        public string OriginalQuestionText { get; set; }
        public int NewQuestionNumber { get; set; }
        public string NewQuestionText { get; set; }
        public IEnumerable<int> DeletedOptions { get; set; }
        public IEnumerable<EditableOption> EditedOptions { get; set; }
    }
}