using System;
using System.Collections.Generic;
using System.Text;

namespace ApiService.Entity
{
    public class UserExamAttemptInfo
    {
        public int Id { get; set; }
        public int User_Id { get; set; }
        public int Exam_Id { get; set; }
        public int Attempt_Number { get; set; }
        public DateTime Start_Time { get; set; }
        public DateTime? End_Time { get; set; }
        public decimal Total_Score { get; set; }
        public bool Passed { get; set; }
        public DateTime Created_At { get; set; }
        public DateTime Updated_At { get; set; }
    }
} 