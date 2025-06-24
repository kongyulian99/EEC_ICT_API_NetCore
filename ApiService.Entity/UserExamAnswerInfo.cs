using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace ApiService.Entity
{
    public class UserExamAnswerInfo
    {
        public int Id { get; set; }
        public int User_Id { get; set; }
        public int Exam_Id { get; set; }
        public int Question_Id { get; set; }
        public int Attempt_Number { get; set; }
        public string Answer_Given_Json { get; set; }
        public bool? Is_Correct { get; set; }
        public decimal Score_Achieved { get; set; }
        public int Time_Spent_Seconds { get; set; }
        public DateTime Created_At { get; set; }
        public DateTime Updated_At { get; set; }
    }

    public class MultipleChoiceAnswerJsonInfo
    {
        public List<string> options { get; set; }
        public int correctOption { get; set; }
    }
    
    public class FillInTheBlankAnswerJsonInfo
    {
        public List<string> answers { get; set; }
        public List<string> segments { get; set; }
        
        /// <summary>
        /// Cấu trúc JSON tương ứng:
        /// {"answers": ["hay", "ta", "từ"], "segments": ["<p></p>", " là đôi ", " hẹn ước ", " hư vô a<sup>2</sup><p></p>"]}
        /// </summary>
    }

    public class TrueFalseAnswerJsonInfo {
        public bool correctAnswer { get; set; }
        
        /// <summary>
        /// Cấu trúc JSON tương ứng:
        /// {"correctAnswer": true}
        /// </summary>
    }
    
} 