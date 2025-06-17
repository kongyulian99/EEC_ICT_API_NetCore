using System;
using System.Text.Json;

namespace ApiService.Entity
{
    public class QuestionInfo
    {
        public int Id { get; set; }
        public int Topic_Id { get; set; }
        public int Exam_Id { get; set; }
        public QuestionType Question_Type { get; set; }
        public string Question_Type_Name { get; set; }
        public string Content { get; set; }
        public string Question_Data_Json { get; set; }
        //public string Question_Data_Json { get; set; }
        public string Explanation { get; set; }
        public Enum_DifficutyLevel Difficulty_Level { get; set; }
        public string Difficulty_Level_Name { get; set; }
        public DateTime Created_At { get; set; }
        public DateTime Updated_At { get; set; }
    }
} 