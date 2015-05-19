using System.Collections.Generic;

namespace CUWebinars.Business.Core
{
    public struct QuizScore
    {
        public int TotalQuestionCount { get; set; }
        public int TotalCorrectAnswerCount { get; set; }
        public IDictionary<int, KeyValuePair<char, bool>> QuestionResult { get; set; }
    }
}