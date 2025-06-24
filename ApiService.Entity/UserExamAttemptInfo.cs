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

    /// <summary>
    /// Model cho request chấm điểm bài thi
    /// </summary>
    public class ScoreExamRequest
    {
        public int UserId { get; set; }
        public int ExamId { get; set; }
        public int? AttemptId { get; set; } // ID lần làm bài (nếu đã có)
        public int? AttemptNumber { get; set; } // Số lần làm bài (khi chấm điểm lần làm bài có sẵn)
        public List<UserAnswer> Answers { get; set; } = new List<UserAnswer>(); // Danh sách câu trả lời (khi chấm điểm từ câu trả lời mới)
    }

    /// <summary>
    /// Model cho câu trả lời của người dùng
    /// </summary>
    public class UserAnswer
    {
        public int QuestionId { get; set; }
        public string AnswerGivenJson { get; set; }
        public int TimeSpentSeconds { get; set; }
    }

    /// <summary>
    /// Model cho request lấy lịch sử làm bài
    /// </summary>
    public class UserExamHistoryRequest
    {
        public int UserId { get; set; }
        public int? ExamId { get; set; }  // Nếu null, sẽ lấy lịch sử tất cả các bài thi
        public int? LimitRecentAttempts { get; set; }  // Giới hạn số lần làm bài gần nhất cho mỗi bài thi
    }

    /// <summary>
    /// Model cho mỗi item trong lịch sử làm bài
    /// </summary>
    public class UserExamHistoryItem
    {
        public int AttemptId { get; set; }
        public int AttemptNumber { get; set; }
        public int ExamId { get; set; }
        public string ExamTitle { get; set; }
        public int TotalQuestions { get; set; }
        public decimal Score { get; set; }
        public decimal PassScore { get; set; }
        public bool Passed { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public int DurationMinutes { get; set; }
        public string DurationFormatted { get; set; }
        public string CompletionStatus { get; set; }
    }
} 