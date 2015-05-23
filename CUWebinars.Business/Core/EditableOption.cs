namespace CUWebinars.Business.Core
{
    public enum EditType
    {
        Added = 0,
        Edited = 1,
        Deleted = 2
    }
    
    public class EditableOption
    {
        public EditType EditType { get; set; }
        public int OptionId { get; set; }
        public string OriginalOptionLetter { get; set; }
        public string NewOptionLetter { get; set; }
        public string OriginalOptionText { get; set; }
        public string NewOptionText { get; set; }
        public bool OriginalCorrectStatus { get; set; }
        public bool NewCorrectStatus { get; set; }
    }
}