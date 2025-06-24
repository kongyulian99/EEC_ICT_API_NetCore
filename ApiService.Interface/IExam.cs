using ApiService.Entity;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ApiService.Interface
{
    public interface IExam
    {
        Task<int> CreateExam(ExamInfo exam);
        Task<bool> UpdateExam(ExamInfo exam);
        Task<bool> DeleteExam(int id);
        Task<ExamInfo> GetExamById(int id);
        Task<IEnumerable<ExamInfo>> GetAllExams();
    }
} 