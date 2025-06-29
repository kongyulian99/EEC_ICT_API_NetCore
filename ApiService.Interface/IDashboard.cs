using ApiService.Entity;
using System;
using System.Collections.Generic;
using System.Data;

namespace ApiService.Interface
{
    public interface IDashboard
    {
        DbReturnInfo<DashboardSummary> GetSystemSummary();
        DbReturnInfo<List<ExamAttemptsOverTime>> GetExamAttemptsOverTime(DateTime? fromDate, DateTime? toDate);
        DbReturnInfo<List<ScoreDistribution>> GetScoreDistribution(int? examId);
        DbReturnInfo<List<TopExamByPassRate>> GetTopExamsByPassRate(int limit = 10);
        DbReturnInfo<List<RecentActivity>> GetRecentActivities(int limit = 20);
    }

    public class DashboardSummary
    {
        public int TotalUsers { get; set; }
        public int TotalExams { get; set; }
        public int TotalQuestions { get; set; }
        public int TotalAttempts { get; set; }
        public decimal OverallPassRate { get; set; }
    }

    public class ExamAttemptsOverTime
    {
        public DateTime Date { get; set; }
        public int TotalAttempts { get; set; }
        public int PassedAttempts { get; set; }
    }

    public class ScoreDistribution
    {
        public string ScoreRange { get; set; }
        public int Count { get; set; }
    }

    public class TopExamByPassRate
    {
        public int ExamId { get; set; }
        public string ExamTitle { get; set; }
        public int TotalAttempts { get; set; }
        public int PassedAttempts { get; set; }
        public decimal PassRate { get; set; }
    }

    public class RecentActivity
    {
        public int Id { get; set; }
        public string ActivityType { get; set; } // "exam_attempt", "user_registration", etc.
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string Description { get; set; }
        public DateTime Timestamp { get; set; }
        public int? RelatedEntityId { get; set; } // ExamId, QuestionId, etc.
        public string RelatedEntityName { get; set; } // Exam title, etc.
    }
} 