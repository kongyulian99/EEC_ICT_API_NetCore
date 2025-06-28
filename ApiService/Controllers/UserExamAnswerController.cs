using ApiService.Business;
using ApiService.Common;
using ApiService.Entity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserExamAnswerController : MyBaseController<UserExamAnswerController>
    {
        private readonly IMemoryCache _memoryCache;

        public UserExamAnswerController(
            //DI default service
            ILogger<UserExamAnswerController> logger,
            AppSetting appSettingInfo,
            //DI services
            IMemoryCache memoryCache
         ) : base(logger, appSettingInfo)
        {
            _memoryCache = memoryCache;
        }
        
        /// <summary>
        /// Thêm câu trả lời mới cho một câu hỏi trong bài thi
        /// </summary>
        [HttpPost("Create")]
        public async Task<IActionResult> InsertUserExamAnswer([FromBody] UserExamAnswerInfo model)
        {
            var retval = new ReturnBaseInfo<int>();
            retval.ReturnStatus = new StatusBaseInfo { Message = "Thất bại", Code = 0 };
            
            if (model == null || model.User_Id <= 0 || model.Exam_Id <= 0 || model.Question_Id <= 0 || model.Attempt_Number <= 0)
            {
                retval.ReturnStatus.Message = "Dữ liệu không hợp lệ";
                return Ok(retval);
            }
            
            var answerId = await ServiceFactory.UserExamAnswer.InsertUserExamAnswer(
                model.User_Id,
                model.Exam_Id,
                model.Question_Id,
                model.Attempt_Number,
                model.Answer_Given_Json,
                model.Time_Spent_Seconds
            );
            
            if (answerId > 0)
            {
                retval.ReturnData = answerId;
                retval.ReturnStatus.Code = 1;
                retval.ReturnStatus.Message = "Thêm câu trả lời thành công";
            }
            else
            {
                retval.ReturnStatus.Message = "Không thể thêm câu trả lời";
            }
            
            return Ok(retval);
        }
        
        /// <summary>
        /// Lấy tất cả câu trả lời của một lần làm bài thi
        /// </summary>
        [HttpGet("GetByAttempt/{userId}/{examId}/{attemptNumber}")]
        public async Task<IActionResult> GetUserExamAnswersByAttempt(int userId, int examId, int attemptNumber)
        {
            var retval = new ReturnBaseInfo<IEnumerable<UserExamAnswerInfo>>();
            retval.ReturnStatus = new StatusBaseInfo { Message = "Thất bại", Code = 0 };
            
            if (userId <= 0 || examId <= 0 || attemptNumber <= 0)
            {
                retval.ReturnStatus.Message = "Dữ liệu không hợp lệ";
                return Ok(retval);
            }
            
            var answers = await ServiceFactory.UserExamAnswer.GetUserExamAnswersByAttempt(userId, examId, attemptNumber);
            if (answers != null)
            {
                retval.ReturnData = answers;
                retval.ReturnStatus.Code = 1;
                retval.ReturnStatus.Message = "Lấy danh sách câu trả lời thành công";
            }
            else
            {
                retval.ReturnStatus.Message = "Không thể lấy danh sách câu trả lời";
            }
            
            return Ok(retval);
        }
        
        /// <summary>
        /// Cập nhật câu trả lời
        /// </summary>
        [HttpPut("Update")]
        public async Task<IActionResult> UpdateUserExamAnswer([FromBody] UserExamAnswerInfo model)
        {
            var retval = new ReturnBaseInfo<bool>();
            retval.ReturnStatus = new StatusBaseInfo { Message = "Thất bại", Code = 0 };
            
            if (model == null || model.Id <= 0)
            {
                retval.ReturnStatus.Message = "Dữ liệu không hợp lệ";
                return Ok(retval);
            }
            
            var result = await ServiceFactory.UserExamAnswer.UpdateUserExamAnswer(
                model.Id,
                model.Answer_Given_Json,
                model.Is_Correct ?? false,
                model.Score_Achieved,
                model.Time_Spent_Seconds
            );
            
            if (result)
            {
                retval.ReturnData = true;
                retval.ReturnStatus.Code = 1;
                retval.ReturnStatus.Message = "Cập nhật câu trả lời thành công";
            }
            else
            {
                retval.ReturnStatus.Message = "Không thể cập nhật câu trả lời";
            }
            
            return Ok(retval);
        }
        
        /// <summary>
        /// Xóa câu trả lời
        /// </summary>
        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> DeleteUserExamAnswer(int id)
        {
            var retval = new ReturnBaseInfo<bool>();
            retval.ReturnStatus = new StatusBaseInfo { Message = "Thất bại", Code = 0 };
            
            if (id <= 0)
            {
                retval.ReturnStatus.Message = "ID không hợp lệ";
                return Ok(retval);
            }
            
            var result = await ServiceFactory.UserExamAnswer.DeleteUserExamAnswer(id);
            if (result)
            {
                retval.ReturnData = true;
                retval.ReturnStatus.Code = 1;
                retval.ReturnStatus.Message = "Xóa câu trả lời thành công";
            }
            else
            {
                retval.ReturnStatus.Message = "Không thể xóa câu trả lời";
            }
            
            return Ok(retval);
        }
        
        /// <summary>
        /// Lấy danh sách 10 topic có điểm trung bình thấp nhất dựa trên kết quả trả lời của người dùng
        /// </summary>
        [HttpGet("GetLowestPerformingTopics")]
        public async Task<IActionResult> GetLowestPerformingTopics([FromQuery] int? userId = null, [FromQuery] int limit = 10)
        {
            var retval = new ReturnBaseInfo<IEnumerable<TopicPerformanceInfo>>();
            retval.ReturnStatus = new StatusBaseInfo { Message = "Thất bại", Code = 0 };
            
            try
            {
                // 1. Lấy tất cả các topic
                var allTopicsResult = ServiceFactory.Topic.GetAllTopics();
                if (allTopicsResult.Code != 1 || allTopicsResult.Result == null)
                {
                    retval.ReturnStatus.Message = "Không thể lấy danh sách topic";
                    return Ok(retval);
                }
                
                var allTopics = allTopicsResult.Result;
                var topicPerformanceList = new List<TopicPerformanceInfo>();
                
                // 2. Lấy tất cả câu hỏi để biết thuộc topic nào
                var allQuestions = await ServiceFactory.Question.GetAllQuestions();
                if (allQuestions == null)
                {
                    retval.ReturnStatus.Message = "Không thể lấy danh sách câu hỏi";
                    return Ok(retval);
                }
                
                // Nhóm câu hỏi theo topic
                var questionsByTopic = allQuestions.GroupBy(q => q.Topic_Id)
                    .ToDictionary(g => g.Key, g => g.ToList());
                
                // 3. Lấy tất cả câu trả lời của người dùng
                IEnumerable<UserExamAnswerInfo> allAnswers;
                if (userId.HasValue)
                {
                    // Nếu chỉ định userId, chỉ lấy câu trả lời của người dùng đó
                    allAnswers = await ServiceFactory.UserExamAnswer.GetUserExamAnswersByUserId(userId.Value);
                }
                else
                {
                    // Nếu không chỉ định userId, lấy tất cả câu trả lời
                    allAnswers = [];
                }
                
                if (allAnswers == null)
                {
                    retval.ReturnStatus.Message = "Không thể lấy danh sách câu trả lời";
                    return Ok(retval);
                }
                
                // 4. Tính toán điểm trung bình cho mỗi topic
                foreach (var topic in allTopics)
                {
                    // Kiểm tra xem topic có câu hỏi không
                    if (!questionsByTopic.ContainsKey(topic.Id))
                    {
                        continue; // Bỏ qua topic không có câu hỏi
                    }
                    
                    var topicQuestions = questionsByTopic[topic.Id];
                    
                    // Tính tổng điểm tối đa có thể đạt được
                    decimal totalMaxScore = topicQuestions.Sum(q => q.Score);
                    
                    // Tính điểm đạt được cho mỗi câu hỏi (lấy điểm cao nhất nếu có nhiều lần làm)
                    decimal totalAchievedScore = 0;
                    int answeredQuestionCount = 0;
                    
                    foreach (var question in topicQuestions)
                    {
                        // Lấy tất cả câu trả lời cho câu hỏi này
                        var questionAnswers = allAnswers.Where(a => a.Question_Id == question.Id);
                        
                        if (questionAnswers.Any())
                        {
                            // Lấy điểm cao nhất đạt được cho câu hỏi này
                            decimal highestScore = questionAnswers.Max(a => a.Score_Achieved);
                            totalAchievedScore += highestScore;
                            answeredQuestionCount++;
                        }
                    }
                    
                    // Chỉ tính điểm trung bình nếu có câu hỏi được trả lời
                    if (answeredQuestionCount > 0)
                    {
                        // Tính điểm trung bình theo phần trăm
                        decimal maxPossibleScore = topicQuestions.Count > 0 ? totalMaxScore : 1;
                        decimal percentageScore = (totalAchievedScore / maxPossibleScore) * 100;
                        
                        topicPerformanceList.Add(new TopicPerformanceInfo
                        {
                            TopicId = topic.Id,
                            TopicName = topic.Name,
                            TotalQuestions = topicQuestions.Count,
                            AnsweredQuestions = answeredQuestionCount,
                            AverageScore = Math.Round(percentageScore, 2),
                            MaxPossibleScore = totalMaxScore,
                            TotalAchievedScore = totalAchievedScore
                        });
                    }
                }
                
                // 5. Sắp xếp theo điểm trung bình tăng dần và lấy số lượng topic theo limit
                var lowestPerformingTopics = topicPerformanceList
                    .OrderBy(t => t.AverageScore)
                    .Take(limit)
                    .ToList();
                
                retval.ReturnData = lowestPerformingTopics;
                retval.ReturnStatus.Code = 1;
                retval.ReturnStatus.Message = "Lấy danh sách topic điểm thấp nhất thành công";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy danh sách topic điểm thấp nhất");
                retval.ReturnStatus.Message = "Lỗi xử lý: " + ex.Message;
            }
            
            return Ok(retval);
        }
    }
    
    /// <summary>
    /// Thông tin hiệu suất của một topic
    /// </summary>
    public class TopicPerformanceInfo
    {
        /// <summary>
        /// ID của topic
        /// </summary>
        public int TopicId { get; set; }
        
        /// <summary>
        /// Tên của topic
        /// </summary>
        public string TopicName { get; set; }
        
        /// <summary>
        /// Tổng số câu hỏi thuộc topic
        /// </summary>
        public int TotalQuestions { get; set; }
        
        /// <summary>
        /// Số câu hỏi đã được trả lời
        /// </summary>
        public int AnsweredQuestions { get; set; }
        
        /// <summary>
        /// Điểm trung bình (theo phần trăm)
        /// </summary>
        public decimal AverageScore { get; set; }
        
        /// <summary>
        /// Điểm tối đa có thể đạt được
        /// </summary>
        public decimal MaxPossibleScore { get; set; }
        
        /// <summary>
        /// Tổng điểm đạt được
        /// </summary>
        public decimal TotalAchievedScore { get; set; }
    }
} 