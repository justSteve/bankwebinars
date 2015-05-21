namespace CUWebinars.Business.Core
{
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