using ApiService.Entity;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ApiService.Interface
{
    public interface IUserExamAnswer
    {
        Task<int> InsertUserExamAnswer(int userId, int examId, int questionId, int attemptNumber, string answerGivenJson, int timeSpentSeconds);
        Task<IEnumerable<UserExamAnswerInfo>> GetUserExamAnswersByAttempt(int userId, int examId, int attemptNumber);
        Task<bool> UpdateUserExamAnswer(int id, string answerGivenJson, bool isCorrect, decimal scoreAchieved, int timeSpentSeconds);
        Task<bool> DeleteUserExamAnswer(int id);
    }
} 