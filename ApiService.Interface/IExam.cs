using ApiService.Entity;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ApiService.Interface
{
    public interface IExam
    {
        Task<int> CreateExam(string title, string description, int durationMinutes, int totalQuestions);
        Task<bool> UpdateExam(int id, string title, string description, int durationMinutes, int totalQuestions);
        Task<bool> DeleteExam(int id);
        Task<ExamInfo> GetExamById(int id);
        Task<IEnumerable<ExamInfo>> GetAllExams();
    }
} 