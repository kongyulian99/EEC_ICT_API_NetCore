using ApiService.Entity;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;

namespace ApiService.Interface
{
    public interface IQuestion
    {
        Task<int> CreateQuestion(QuestionInfo question);
            
        Task<bool> UpdateQuestion(QuestionInfo question);
            
        Task<bool> DeleteQuestion(int id);
        
        Task<QuestionInfo> GetQuestionById(int id);
        
        Task<IEnumerable<QuestionInfo>> GetQuestionsByTopicId(int topicId);
        
        Task<IEnumerable<QuestionInfo>> GetQuestionsByExamId(int examId);
        
        Task<IEnumerable<QuestionInfo>> GetQuestionsByDifficultyLevel(Enum_DifficutyLevel difficultyLevel);
        
        Task<IEnumerable<QuestionInfo>> GetAllQuestions();
    }
} 