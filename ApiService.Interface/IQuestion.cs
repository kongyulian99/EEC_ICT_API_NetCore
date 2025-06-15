using ApiService.Entity;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;

namespace ApiService.Interface
{
    public interface IQuestion
    {
        Task<int> CreateQuestion(int topicId, int examId, QuestionType questionType, string content, 
            JsonDocument questionDataJson, string explanation, Enum_DifficutyLevel difficultyLevel);
            
        Task<bool> UpdateQuestion(int id, int topicId, int examId, QuestionType questionType, string content,
            JsonDocument questionDataJson, string explanation, Enum_DifficutyLevel difficultyLevel);
            
        Task<bool> DeleteQuestion(int id);
        
        Task<QuestionInfo> GetQuestionById(int id);
        
        Task<IEnumerable<QuestionInfo>> GetQuestionsByTopicId(int topicId);
        
        Task<IEnumerable<QuestionInfo>> GetQuestionsByExamId(int examId);
        
        Task<IEnumerable<QuestionInfo>> GetQuestionsByDifficultyLevel(Enum_DifficutyLevel difficultyLevel);
        
        Task<IEnumerable<QuestionInfo>> GetAllQuestions();
    }
} 