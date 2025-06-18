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
    public class UserExamAttemptController : MyBaseController<UserExamAttemptController>
    {
        private readonly IMemoryCache _memoryCache;

        public UserExamAttemptController(
            //DI default service
            ILogger<UserExamAttemptController> logger,
            AppSetting appSettingInfo,
            //DI services
            IMemoryCache memoryCache
         ) : base(logger, appSettingInfo)
        {
            _memoryCache = memoryCache;
        }
        
        /// <summary>
        /// Tạo lần làm bài thi mới cho người dùng
        /// </summary>
        [HttpPost("Create")]
        public async Task<IActionResult> InsertUserExamAttempt([FromBody] UserExamAttemptInfo model)
        {
            var retval = new ReturnBaseInfo<object>();
            retval.ReturnStatus = new StatusBaseInfo { Message = "Thất bại", Code = 0 };
            
            if (model == null || model.User_Id <= 0 || model.Exam_Id <= 0)
            {
                retval.ReturnStatus.Message = "Dữ liệu không hợp lệ";
                return Ok(retval);
            }
            
            var result = await ServiceFactory.UserExamAttempt.InsertUserExamAttempt(model.User_Id, model.Exam_Id);
            if (result.NewAttemptId > 0)
            {
                retval.ReturnData = new { 
                    NewAttemptId = result.NewAttemptId, 
                    AttemptNumber = result.AttemptNumber 
                };
                retval.ReturnStatus.Code = 1;
                retval.ReturnStatus.Message = "Tạo lần làm bài thi thành công";
            }
            else
            {
                retval.ReturnStatus.Message = "Không thể tạo lần làm bài thi";
            }
            
            return Ok(retval);
        }
        
        /// <summary>
        /// Lấy thông tin lần làm bài thi theo ID
        /// </summary>
        [HttpGet("GetById/{id}")]
        public async Task<IActionResult> GetUserExamAttemptById(int id)
        {
            var retval = new ReturnBaseInfo<UserExamAttemptInfo>();
            retval.ReturnStatus = new StatusBaseInfo { Message = "Thất bại", Code = 0 };
            
            if (id <= 0)
            {
                retval.ReturnStatus.Message = "ID không hợp lệ";
                return Ok(retval);
            }
            
            var attempt = await ServiceFactory.UserExamAttempt.GetUserExamAttemptById(id);
            if (attempt != null)
            {
                retval.ReturnData = attempt;
                retval.ReturnStatus.Code = 1;
                retval.ReturnStatus.Message = "Lấy thông tin lần làm bài thi thành công";
            }
            else
            {
                retval.ReturnStatus.Message = "Không tìm thấy lần làm bài thi";
            }
            
            return Ok(retval);
        }
        
        /// <summary>
        /// Lấy danh sách các lần làm bài thi của người dùng cho một bài thi cụ thể
        /// </summary>
        [HttpGet("GetByUserAndExam/{userId}/{examId}")]
        public async Task<IActionResult> GetUserExamAttemptsByUserIdAndExamId(int userId, int examId)
        {
            var retval = new ReturnBaseInfo<IEnumerable<UserExamAttemptInfo>>();
            retval.ReturnStatus = new StatusBaseInfo { Message = "Thất bại", Code = 0 };
            
            if (userId <= 0 || examId <= 0)
            {
                retval.ReturnStatus.Message = "Dữ liệu không hợp lệ";
                return Ok(retval);
            }
            
            var attempts = await ServiceFactory.UserExamAttempt.GetUserExamAttemptsByUserIdAndExamId(userId, examId);
            if (attempts != null)
            {
                retval.ReturnData = attempts;
                retval.ReturnStatus.Code = 1;
                retval.ReturnStatus.Message = "Lấy danh sách lần làm bài thi thành công";
            }
            else
            {
                retval.ReturnStatus.Message = "Không thể lấy danh sách lần làm bài thi";
            }
            
            return Ok(retval);
        }
        
        /// <summary>
        /// Cập nhật thông tin lần làm bài thi
        /// </summary>
        [HttpPut("Update")]
        public async Task<IActionResult> UpdateUserExamAttempt([FromBody] UserExamAttemptInfo model)
        {
            var retval = new ReturnBaseInfo<bool>();
            retval.ReturnStatus = new StatusBaseInfo { Message = "Thất bại", Code = 0 };
            
            if (model == null || model.Id <= 0)
            {
                retval.ReturnStatus.Message = "Dữ liệu không hợp lệ";
                return Ok(retval);
            }
            
            var result = await ServiceFactory.UserExamAttempt.UpdateUserExamAttempt(
                model.Id, 
                model.End_Time ?? DateTime.Now, 
                model.Total_Score, 
                model.Passed
            );
            
            if (result)
            {
                retval.ReturnData = true;
                retval.ReturnStatus.Code = 1;
                retval.ReturnStatus.Message = "Cập nhật lần làm bài thi thành công";
            }
            else
            {
                retval.ReturnStatus.Message = "Không thể cập nhật lần làm bài thi";
            }
            
            return Ok(retval);
        }
        
        /// <summary>
        /// Xóa lần làm bài thi
        /// </summary>
        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> DeleteUserExamAttempt(int id)
        {
            var retval = new ReturnBaseInfo<bool>();
            retval.ReturnStatus = new StatusBaseInfo { Message = "Thất bại", Code = 0 };
            
            if (id <= 0)
            {
                retval.ReturnStatus.Message = "ID không hợp lệ";
                return Ok(retval);
            }
            
            var result = await ServiceFactory.UserExamAttempt.DeleteUserExamAttempt(id);
            if (result)
            {
                retval.ReturnData = true;
                retval.ReturnStatus.Code = 1;
                retval.ReturnStatus.Message = "Xóa lần làm bài thi thành công";
            }
            else
            {
                retval.ReturnStatus.Message = "Không thể xóa lần làm bài thi";
            }
            
            return Ok(retval);
        }
        
        /// <summary>
        /// Chấm điểm cho một lần làm bài thi
        /// </summary>
        [HttpPost("Score")]
        public async Task<IActionResult> ScoreUserExamAttempt([FromBody] UserExamAttemptInfo model)
        {
            var retval = new ReturnBaseInfo<object>();
            retval.ReturnStatus = new StatusBaseInfo { Message = "Thất bại", Code = 0 };
            
            if (model == null || model.User_Id <= 0 || model.Exam_Id <= 0 || model.Attempt_Number <= 0)
            {
                retval.ReturnStatus.Message = "Dữ liệu không hợp lệ";
                return Ok(retval);
            }
            
            var result = await ServiceFactory.UserExamAttempt.ScoreUserExamAttempt(
                model.User_Id, 
                model.Exam_Id, 
                model.Attempt_Number
            );
            
            if (result.FinalScore >= 0)
            {
                retval.ReturnData = new { 
                    FinalScore = result.FinalScore, 
                    Passed = result.Passed 
                };
                retval.ReturnStatus.Code = 1;
                retval.ReturnStatus.Message = "Chấm điểm lần làm bài thi thành công";
            }
            else
            {
                retval.ReturnStatus.Message = "Không thể chấm điểm lần làm bài thi";
            }
            
            return Ok(retval);
        }
        
        /// <summary>
        /// Chấm điểm tổng thể cho bài kiểm tra và trả về kết quả chi tiết
        /// </summary>
        [HttpGet("ScoreExam/{userId}/{examId}/{attemptNumber}")]
        public async Task<IActionResult> ScoreCompleteExam(int userId, int examId, int attemptNumber)
        {
            var retval = new ReturnBaseInfo<object>();
            retval.ReturnStatus = new StatusBaseInfo { Message = "Thất bại", Code = 0 };
            
            if (userId <= 0 || examId <= 0 || attemptNumber <= 0)
            {
                retval.ReturnStatus.Message = "Dữ liệu không hợp lệ";
                return Ok(retval);
            }
            
            try
            {
                // 1. Chấm điểm bài thi
                var scoreResult = await ServiceFactory.UserExamAttempt.ScoreUserExamAttempt(userId, examId, attemptNumber);
                
                // 2. Lấy thông tin chi tiết về bài thi
                var examInfo = await ServiceFactory.Exam.GetExamById(examId);
                
                // 3. Lấy tất cả các câu trả lời của người dùng
                var userAnswers = await ServiceFactory.UserExamAnswer.GetUserExamAnswersByAttempt(userId, examId, attemptNumber);
                
                // 4. Lấy thông tin về lần làm bài thi
                var attemptInfo = await ServiceFactory.UserExamAttempt.GetUserExamAttemptById(
                    (await ServiceFactory.UserExamAttempt.GetUserExamAttemptsByUserIdAndExamId(userId, examId))
                    .FirstOrDefault(a => a.Attempt_Number == attemptNumber)?.Id ?? 0
                );
                
                if (scoreResult.FinalScore >= 0 && examInfo != null && userAnswers != null && attemptInfo != null)
                {
                    // Tính toán thêm các thông tin thống kê
                    int totalQuestions = userAnswers.Count();
                    int correctAnswers = userAnswers.Count(a => a.Is_Correct == true);
                    int incorrectAnswers = userAnswers.Count(a => a.Is_Correct == false);
                    int unansweredQuestions = totalQuestions - (correctAnswers + incorrectAnswers);
                    
                    // Tính thời gian làm bài
                    TimeSpan duration = attemptInfo.End_Time.HasValue 
                        ? attemptInfo.End_Time.Value - attemptInfo.Start_Time 
                        : DateTime.Now - attemptInfo.Start_Time;
                    
                    retval.ReturnData = new
                    {
                        ExamId = examId,
                        ExamTitle = examInfo.Title,
                        UserId = userId,
                        AttemptNumber = attemptNumber,
                        FinalScore = scoreResult.FinalScore,
                        Passed = scoreResult.Passed,
                        PassScore = examInfo.Pass_Score,
                        TotalQuestions = totalQuestions,
                        CorrectAnswers = correctAnswers,
                        IncorrectAnswers = incorrectAnswers,
                        UnansweredQuestions = unansweredQuestions,
                        StartTime = attemptInfo.Start_Time,
                        EndTime = attemptInfo.End_Time,
                        Duration = $"{duration.Hours:D2}:{duration.Minutes:D2}:{duration.Seconds:D2}",
                        TotalTimeInSeconds = (int)duration.TotalSeconds,
                        DetailedAnswers = userAnswers
                    };
                    
                    retval.ReturnStatus.Code = 1;
                    retval.ReturnStatus.Message = "Chấm điểm và lấy kết quả chi tiết thành công";
                }
                else
                {
                    retval.ReturnStatus.Message = "Không thể lấy đầy đủ thông tin để chấm điểm";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi chấm điểm bài thi");
                retval.ReturnStatus.Message = "Lỗi xử lý: " + ex.Message;
            }
            
            return Ok(retval);
        }
        
        /// <summary>
        /// Chấm điểm bài thi dựa trên danh sách câu trả lời của người dùng
        /// </summary>
        [HttpPost("ScoreWithAnswers")]
        public async Task<IActionResult> ScoreExamWithAnswers([FromBody] ScoreExamRequest request)
        {
            var retval = new ReturnBaseInfo<object>();
            retval.ReturnStatus = new StatusBaseInfo { Message = "Thất bại", Code = 0 };
            
            if (request == null || request.UserId <= 0 || request.ExamId <= 0 || 
                request.Answers == null || !request.Answers.Any())
            {
                retval.ReturnStatus.Message = "Dữ liệu không hợp lệ";
                return Ok(retval);
            }
            
            try
            {
                // 1. Tạo hoặc lấy lần làm bài thi hiện tại
                int attemptNumber;
                int attemptId;
                
                if (request.AttemptId.HasValue && request.AttemptId.Value > 0)
                {
                    // Sử dụng lần làm bài đã có
                    var existingAttempt = await ServiceFactory.UserExamAttempt.GetUserExamAttemptById(request.AttemptId.Value);
                    if (existingAttempt == null)
                    {
                        retval.ReturnStatus.Message = "Không tìm thấy lần làm bài thi";
                        return Ok(retval);
                    }
                    
                    attemptId = existingAttempt.Id;
                    attemptNumber = existingAttempt.Attempt_Number;
                }
                else
                {
                    // Tạo lần làm bài mới
                    var newAttempt = await ServiceFactory.UserExamAttempt.InsertUserExamAttempt(
                        request.UserId, 
                        request.ExamId
                    );
                    
                    attemptId = newAttempt.NewAttemptId;
                    attemptNumber = newAttempt.AttemptNumber;
                    
                    if (attemptId <= 0)
                    {
                        retval.ReturnStatus.Message = "Không thể tạo lần làm bài thi mới";
                        return Ok(retval);
                    }
                }
                
                // 2. Lấy thông tin về đề thi
                var examInfo = await ServiceFactory.Exam.GetExamById(request.ExamId);
                if (examInfo == null)
                {
                    retval.ReturnStatus.Message = "Không tìm thấy thông tin đề thi";
                    return Ok(retval);
                }
                
                // 3. Lưu từng câu trả lời và tính điểm
                decimal totalScore = 0;
                int correctAnswers = 0;
                List<UserAnswerResult> answerResults = new List<UserAnswerResult>();
                
                foreach (var answer in request.Answers)
                {
                    // Lưu câu trả lời vào database
                    int answerId = await ServiceFactory.UserExamAnswer.InsertUserExamAnswer(
                        request.UserId,
                        request.ExamId,
                        answer.QuestionId,
                        attemptNumber,
                        answer.AnswerGivenJson,
                        answer.TimeSpentSeconds
                    );
                    
                    // Nếu lưu thành công, tiếp tục xử lý
                    if (answerId > 0)
                    {
                        // Lấy thông tin câu hỏi để chấm điểm
                        var question = await ServiceFactory.Question.GetQuestionById(answer.QuestionId);
                        
                        // Chấm điểm câu hỏi (logic đơn giản, có thể mở rộng sau)
                        bool isCorrect = false;
                        decimal scoreAchieved = 0;
                        
                        if (question != null)
                        {
                            // Logic chấm điểm dựa trên loại câu hỏi
                            // Chú ý: Đây là logic đơn giản, có thể cần điều chỉnh theo cấu trúc dữ liệu thực tế
                            switch (question.Question_Type)
                            {
                                case QuestionType.MULTIPLE_CHOICE:
                                    // Giả sử Question_Data_Json có dạng {"correctOption": "A"} và Answer_Given_Json có dạng {"selected_option_id": "A"}
                                    var correctOption = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(question.Question_Data_Json)?.correctOption?.ToString();
                                    var selectedOption = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(answer.AnswerGivenJson)?.selected_option_id?.ToString();
                                    
                                    isCorrect = !string.IsNullOrEmpty(correctOption) && 
                                               !string.IsNullOrEmpty(selectedOption) && 
                                               correctOption.Equals(selectedOption, StringComparison.OrdinalIgnoreCase);
                                    break;
                                    
                                case QuestionType.FILL_IN_THE_BLANK:
                                    // Giả sử Question_Data_Json có dạng {"correct_answer": "answer"} và Answer_Given_Json có dạng {"text_input": "answer"}
                                    var correctAnswer = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(question.Question_Data_Json)?.answers?.ToString();
                                    var userAnswer = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(answer.AnswerGivenJson)?.answers?.ToString();
                                    
                                    isCorrect = !string.IsNullOrEmpty(correctAnswer) && 
                                               !string.IsNullOrEmpty(userAnswer) && 
                                               correctAnswer.Equals(userAnswer, StringComparison.OrdinalIgnoreCase);
                                    break;
                                    
                                case QuestionType.TRUE_FALSE:
                                    // Giả sử Question_Data_Json có dạng {"correct_answer": true} và Answer_Given_Json có dạng {"selected_answer": true}
                                    bool? correctBool = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(question.Question_Data_Json)?.correct_answer;
                                    bool? userBool = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(answer.AnswerGivenJson)?.selected_answer;
                                    
                                    isCorrect = correctBool.HasValue && 
                                               userBool.HasValue && 
                                               correctBool.Value == userBool.Value;
                                    break;
                                    
                                // Thêm các loại câu hỏi khác nếu cần
                                default:
                                    isCorrect = false;
                                    break;
                            }
                            
                            // Tính điểm cho câu hỏi
                            scoreAchieved = isCorrect ? question.Score : 0;
                            
                            // Cập nhật câu trả lời với kết quả chấm điểm
                            await ServiceFactory.UserExamAnswer.UpdateUserExamAnswer(
                                answerId,
                                answer.AnswerGivenJson,
                                isCorrect,
                                scoreAchieved,
                                answer.TimeSpentSeconds
                            );
                            
                            // Cập nhật tổng điểm và số câu đúng
                            if (isCorrect)
                            {
                                correctAnswers++;
                            }
                            totalScore += scoreAchieved;
                            
                            // Thêm kết quả vào danh sách
                            answerResults.Add(new UserAnswerResult
                            {
                                QuestionId = answer.QuestionId,
                                AnswerId = answerId,
                                IsCorrect = isCorrect,
                                ScoreAchieved = scoreAchieved
                            });
                        }
                    }
                }
                
                // 4. Cập nhật kết quả lần làm bài thi
                bool passed = totalScore >= examInfo.Pass_Score;
                await ServiceFactory.UserExamAttempt.UpdateUserExamAttempt(
                    attemptId,
                    DateTime.Now,
                    totalScore,
                    passed
                );
                
                // 5. Trả về kết quả
                retval.ReturnData = new
                {
                    AttemptId = attemptId,
                    AttemptNumber = attemptNumber,
                    UserId = request.UserId,
                    ExamId = request.ExamId,
                    ExamTitle = examInfo.Title,
                    TotalScore = totalScore,
                    PassScore = examInfo.Pass_Score,
                    Passed = passed,
                    TotalQuestions = request.Answers.Count,
                    CorrectAnswers = correctAnswers,
                    IncorrectAnswers = request.Answers.Count - correctAnswers,
                    AnswerResults = answerResults
                };
                
                retval.ReturnStatus.Code = 1;
                retval.ReturnStatus.Message = "Chấm điểm bài thi thành công";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi chấm điểm bài thi");
                retval.ReturnStatus.Message = "Lỗi xử lý: " + ex.Message;
            }
            
            return Ok(retval);
        }
    }
    
    /// <summary>
    /// Model cho request chấm điểm bài thi
    /// </summary>
    public class ScoreExamRequest
    {
        public int UserId { get; set; }
        public int ExamId { get; set; }
        public int? AttemptId { get; set; } // Nếu null, sẽ tạo lần làm bài mới
        public List<UserAnswer> Answers { get; set; } = new List<UserAnswer>();
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
    /// Model cho kết quả chấm điểm của một câu trả lời
    /// </summary>
    public class UserAnswerResult
    {
        public int QuestionId { get; set; }
        public int AnswerId { get; set; }
        public bool IsCorrect { get; set; }
        public decimal ScoreAchieved { get; set; }
    }
} 