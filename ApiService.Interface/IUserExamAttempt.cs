using ApiService.Entity;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ApiService.Interface
{
    public interface IUserExamAttempt
    {
        Task<(int NewAttemptId, int AttemptNumber)> InsertUserExamAttempt(int userId, int examId);
        Task<UserExamAttemptInfo> GetUserExamAttemptById(int id);
        Task<IEnumerable<UserExamAttemptInfo>> GetUserExamAttemptsByUserIdAndExamId(int userId, int examId);
        Task<bool> UpdateUserExamAttempt(int id, DateTime endTime, decimal totalScore, bool passed);
        Task<bool> DeleteUserExamAttempt(int id);
        Task<(decimal FinalScore, bool Passed)> ScoreUserExamAttempt(int userId, int examId, int attemptNumber);
    }
} 