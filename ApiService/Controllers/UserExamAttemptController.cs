using ApiService.Business;
using ApiService.Common;
using ApiService.Entity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
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
            retval.ReturnStatus = new StatusBaseInfo { Message = "Failed", Code = 0 };
            
            if (model == null || model.User_Id <= 0 || model.Exam_Id <= 0)
            {
                retval.ReturnStatus.Message = "Invalid data";
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
                retval.ReturnStatus.Message = "Exam attempt created successfully";
            }
            else
            {
                retval.ReturnStatus.Message = "Could not create exam attempt";
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
        /// Chấm điểm bài thi và trả về kết quả chi tiết.
        /// Hỗ trợ hai phương thức chấm điểm:
        /// 1. Chấm điểm theo thông tin lần làm bài có sẵn (sử dụng userId, examId, attemptNumber)
        /// 2. Chấm điểm từ danh sách câu trả lời mới gửi lên (sử dụng danh sách answers)
        /// </summary>
        [HttpPost("Score")]
        public async Task<IActionResult> ScoreExam([FromBody] ScoreExamRequest request)
        {
            var retval = new ReturnBaseInfo<object>();
            retval.ReturnStatus = new StatusBaseInfo { Message = "Thất bại", Code = 0 };
            
            if (request == null || request.UserId <= 0 || request.ExamId <= 0)
            {
                retval.ReturnStatus.Message = "Dữ liệu không hợp lệ";
                return Ok(retval);
            }
            
            try
            {
                // Xác định phương thức chấm điểm: từ câu trả lời mới hoặc từ lần làm bài có sẵn
                bool scoreFromNewAnswers = request.Answers != null && request.Answers.Any();
                int attemptId;
                int attemptNumber;
                
                // 1. Xử lý lần làm bài
                if (scoreFromNewAnswers)
                {
                    // 1.1 Phương thức 1: Chấm điểm từ danh sách câu trả lời mới
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
                    
                    // Lưu từng câu trả lời mới vào database
                    foreach (var answer in request.Answers)
                    {
                        await ServiceFactory.UserExamAnswer.InsertUserExamAnswer(
                            request.UserId,
                            request.ExamId,
                            answer.QuestionId,
                            attemptNumber,
                            answer.AnswerGivenJson,
                            answer.TimeSpentSeconds
                        );
                    }
                }
                else
                {
                    // 1.2 Phương thức 2: Chấm điểm từ lần làm bài có sẵn
                    if (!request.AttemptNumber.HasValue || request.AttemptNumber.Value <= 0)
                    {
                        retval.ReturnStatus.Message = "Cần cung cấp attemptNumber khi không có danh sách câu trả lời";
                        return Ok(retval);
                    }
                    
                    attemptNumber = request.AttemptNumber.Value;
                    
                    // Lấy ID lần làm bài từ số lần thử
                    var attempts = await ServiceFactory.UserExamAttempt.GetUserExamAttemptsByUserIdAndExamId(
                        request.UserId, 
                        request.ExamId
                    );
                    
                    var currentAttempt = attempts?.FirstOrDefault(a => a.Attempt_Number == attemptNumber);
                    if (currentAttempt == null)
                    {
                        retval.ReturnStatus.Message = "Không tìm thấy lần làm bài thi";
                        return Ok(retval);
                    }
                    
                    attemptId = currentAttempt.Id;
                }
                
                // 2. Lấy thông tin chi tiết
                var examInfo = await ServiceFactory.Exam.GetExamById(request.ExamId);
                if (examInfo == null)
                {
                    retval.ReturnStatus.Message = "Không tìm thấy thông tin bài thi";
                    return Ok(retval);
                }
                
                var userAnswers = await ServiceFactory.UserExamAnswer.GetUserExamAnswersByAttempt(
                    request.UserId, 
                    request.ExamId, 
                    attemptNumber
                );
                
                if (userAnswers == null || !userAnswers.Any())
                {
                    retval.ReturnStatus.Message = "Không tìm thấy câu trả lời cho lần làm bài này";
                    return Ok(retval);
                }
                
                var attemptInfo = await ServiceFactory.UserExamAttempt.GetUserExamAttemptById(attemptId);
                if (attemptInfo == null)
                {
                    retval.ReturnStatus.Message = "Không tìm thấy thông tin lần làm bài";
                    return Ok(retval);
                }
                
                // 3. Chấm điểm từng câu trả lời
                decimal totalScore = 0;
                int correctAnswers = 0;
                int incorrectAnswers = 0;
                
                // Lấy tất cả câu hỏi để chấm điểm
                var questions = await ServiceFactory.Question.GetQuestionsByExamId(request.ExamId);
                var questionDict = questions.ToDictionary(q => q.Id);
                
                foreach (var answer in userAnswers.ToList())
                {
                    if (questionDict.TryGetValue(answer.Question_Id, out var question))
                    {
                        // Chấm điểm câu hỏi
                        bool isCorrect = false;
                        decimal scoreAchieved = 0;
                        
                        // Logic chấm điểm dựa trên loại câu hỏi
                        try
                        {
                            switch (question.Question_Type)
                            {
                                case QuestionType.MULTIPLE_CHOICE:
                                    try
                                    {
                                        // Parse dữ liệu câu hỏi theo đúng định dạng
                                        var mcQuestionData = JsonConvert.DeserializeObject<MultipleChoiceAnswerJsonInfo>(question.Question_Data_Json);
                                        
                                        // Parse dữ liệu câu trả lời
                                        MultipleChoiceAnswerJsonInfo mcUserAnswer;
                                        try
                                        {
                                            // Thử parse trực tiếp theo định dạng mong muốn
                                            mcUserAnswer = JsonConvert.DeserializeObject<MultipleChoiceAnswerJsonInfo>(answer.Answer_Given_Json);
                                        }
                                        catch
                                        {
                                            // Nếu không parse được, thử xử lý dạng dynamic
                                            dynamic rawAnswerData = JsonConvert.DeserializeObject(answer.Answer_Given_Json);
                                            mcUserAnswer = new MultipleChoiceAnswerJsonInfo();
                                            
                                            // Kiểm tra các trường có thể có
                                            if (rawAnswerData.correctOption != null)
                                                mcUserAnswer.correctOption = (int)rawAnswerData.correctOption;
                                            else if (rawAnswerData.selected_option_id != null)
                                            {
                                                if (int.TryParse(rawAnswerData.selected_option_id.ToString(), out int index))
                                                    mcUserAnswer.correctOption = index;
                                            }
                                        }
                                        
                                        // Kiểm tra đáp án
                                        isCorrect = mcQuestionData.correctOption == mcUserAnswer.correctOption;
                                    }
                                    catch (Exception ex)
                                    {
                                        _logger.LogError(ex, $"Lỗi khi parse dữ liệu câu hỏi trắc nghiệm: {ex.Message}");
                                        isCorrect = false;
                                    }
                                    break;
                                    
                                case QuestionType.FILL_IN_THE_BLANK:
                                    try
                                    {
                                        // Parse dữ liệu câu hỏi theo đúng định dạng
                                        var fibQuestionData = JsonConvert.DeserializeObject<FillInTheBlankAnswerJsonInfo>(question.Question_Data_Json);
                                        
                                        // Parse dữ liệu câu trả lời
                                        FillInTheBlankAnswerJsonInfo fibUserAnswer = new FillInTheBlankAnswerJsonInfo();
                                        try
                                        {
                                            // Thử parse trực tiếp theo định dạng mong muốn
                                            fibUserAnswer = JsonConvert.DeserializeObject<FillInTheBlankAnswerJsonInfo>(answer.Answer_Given_Json);
                                        }
                                        catch
                                        {
                                            // Nếu không parse được, thử xử lý dạng dynamic
                                            dynamic rawAnswerData = JsonConvert.DeserializeObject(answer.Answer_Given_Json);
                                            
                                            // Kiểm tra định dạng mảng answers
                                            if (rawAnswerData.answers != null && rawAnswerData.answers.Type == Newtonsoft.Json.Linq.JTokenType.Array)
                                            {
                                                fibUserAnswer.answers = new List<string>();
                                                foreach (var ans in rawAnswerData.answers)
                                                    fibUserAnswer.answers.Add(ans.ToString());
                                            }
                                            // Kiểm tra định dạng text_input đơn
                                            else if (rawAnswerData.text_input != null)
                                            {
                                                fibUserAnswer.answers = new List<string> { rawAnswerData.text_input.ToString() };
                                            }
                                        }
                                        
                                        // Chuẩn hóa đáp án người dùng
                                        List<string> normalizedUserAnswers = new List<string>();
                                        foreach (var ans in fibUserAnswer.answers ?? new List<string>())
                                        {
                                            normalizedUserAnswers.Add(ans.Trim().ToLower());
                                        }
                                            
                                        // Chuẩn hóa đáp án đúng
                                        List<string> normalizedCorrectAnswers = new List<string>();
                                        foreach (var ans in fibQuestionData.answers ?? new List<string>())
                                        {
                                            normalizedCorrectAnswers.Add(ans.Trim().ToLower());
                                        }
                                        
                                        // Tính số lượng chỗ điền đúng
                                        int totalBlanks = normalizedCorrectAnswers.Count;
                                        int correctBlanks = 0;
                                        
                                        if (totalBlanks > 0)
                                        {
                                            // Nếu người dùng trả lời đủ số lượng chỗ trống
                                            if (normalizedUserAnswers.Count == totalBlanks)
                                            {
                                                for (int i = 0; i < totalBlanks; i++)
                                                {
                                                    if (i < normalizedUserAnswers.Count && 
                                                        normalizedUserAnswers[i] == normalizedCorrectAnswers[i])
                                                    {
                                                        correctBlanks++;
                                                    }
                                                }
                                            }
                                            
                                            // Tính điểm dựa trên tỷ lệ chỗ trống điền đúng
                                            decimal percentageCorrect = (decimal)correctBlanks / totalBlanks;
                                            scoreAchieved = question.Score * percentageCorrect;
                                            
                                            // Kiểm tra đúng/sai (isCorrect) dựa trên số chỗ điền đúng
                                            isCorrect = correctBlanks == totalBlanks; // Chỉ đúng khi tất cả chỗ trống đều đúng
                                            
                                            // Bổ sung thông tin chi tiết về câu trả lời
                                            // Lưu thông tin bổ sung về số chỗ điền đúng vào câu trả lời
                                            dynamic extraInfo = new
                                            {
                                                total_blanks = totalBlanks,
                                                correct_blanks = correctBlanks,
                                                percentage_correct = percentageCorrect
                                            };
                                            
                                            // Cập nhật JSON câu trả lời với thông tin bổ sung
                                            dynamic originalAnswerJson = JsonConvert.DeserializeObject(answer.Answer_Given_Json);
                                            if (originalAnswerJson != null)
                                            {
                                                dynamic enrichedAnswerJson = new System.Dynamic.ExpandoObject();
                                                foreach (var prop in originalAnswerJson)
                                                {
                                                    ((IDictionary<string, object>)enrichedAnswerJson)[prop.Name] = prop.Value;
                                                }
                                                ((IDictionary<string, object>)enrichedAnswerJson)["_score_info"] = extraInfo;
                                                
                                                // Cập nhật câu trả lời với kết quả và thông tin bổ sung
                                                await ServiceFactory.UserExamAnswer.UpdateUserExamAnswer(
                                                    answer.Id,
                                                    JsonConvert.SerializeObject(enrichedAnswerJson),
                                                    isCorrect,
                                                    scoreAchieved,
                                                    answer.Time_Spent_Seconds
                                                );
                                                
                                                // Cập nhật tổng điểm và số câu đúng/sai trước khi continue
                                                if (isCorrect)
                                                    correctAnswers++;
                                                else
                                                    incorrectAnswers++;
                                                    
                                                totalScore += scoreAchieved;
                                                
                                                // Bỏ qua lệnh cập nhật mặc định vì đã cập nhật ở trên
                                                continue;
                                            }
                                        }
                                        else
                                        {
                                            // Nếu không có chỗ trống nào cần điền
                                            isCorrect = false;
                                            scoreAchieved = 0;
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        _logger.LogError(ex, $"Lỗi khi parse dữ liệu câu hỏi điền từ: {ex.Message}");
                                        isCorrect = false;
                                        scoreAchieved = 0;
                                    }
                                    break;
                                    
                                case QuestionType.TRUE_FALSE:
                                    try
                                    {
                                        // Parse dữ liệu câu hỏi theo đúng định dạng
                                        var tfQuestionData = JsonConvert.DeserializeObject<TrueFalseAnswerJsonInfo>(question.Question_Data_Json);

                                        // Parse dữ liệu câu trả lời
                                        TrueFalseAnswerJsonInfo tfUserAnswer = new TrueFalseAnswerJsonInfo();
                                        try
                                        {
                                            // Thử parse trực tiếp theo định dạng mong muốn
                                            tfUserAnswer = JsonConvert.DeserializeObject<TrueFalseAnswerJsonInfo>(answer.Answer_Given_Json);
                                        }
                                        catch
                                        {
                                            // Nếu không parse được, thử xử lý dạng dynamic
                                            dynamic rawAnswerData = JsonConvert.DeserializeObject(answer.Answer_Given_Json);
                                            
                                            // Kiểm tra các trường có thể có
                                            if (rawAnswerData.answer != null)
                                                tfUserAnswer.correctAnswer = (bool)rawAnswerData.correctAnswer;
                                            else if (rawAnswerData.selected_answer != null)
                                                tfUserAnswer.correctAnswer = (bool)rawAnswerData.correctAnswer;
                                        }
                                        
                                        // Kiểm tra đáp án
                                        isCorrect = tfQuestionData.correctAnswer == tfUserAnswer.correctAnswer;
                                    }
                                    catch (Exception ex)
                                    {
                                        _logger.LogError(ex, $"Lỗi khi parse dữ liệu câu hỏi đúng/sai: {ex.Message}");
                                        isCorrect = false;
                                    }
                                    break;
                                    
                                default:
                                    isCorrect = false;
                                    break;
                            }
                            
                            // Tính điểm cho câu hỏi
                            scoreAchieved = isCorrect ? question.Score : 0;
                            
                            // Cập nhật câu trả lời với kết quả chấm điểm
                            await ServiceFactory.UserExamAnswer.UpdateUserExamAnswer(
                                answer.Id,
                                answer.Answer_Given_Json,
                                isCorrect,
                                scoreAchieved,
                                answer.Time_Spent_Seconds
                            );
                            
                            // Cập nhật tổng điểm và số câu đúng/sai
                            if (isCorrect)
                                correctAnswers++;
                            else
                                incorrectAnswers++;
                                
                            totalScore += scoreAchieved;
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, $"Lỗi khi chấm điểm câu hỏi ID {question.Id}: {ex.Message}");
                        }
                    }
                }
                
                // 4. Cập nhật kết quả lần làm bài thi
                decimal totalPossibleScore = 0;

                // Tính tổng điểm tối đa có thể đạt được từ tất cả câu hỏi trong bài thi
                foreach (var question in questions)
                {
                    totalPossibleScore += question.Score;
                }

                // Đảm bảo không chia cho 0
                if (totalPossibleScore <= 0)
                {
                    totalPossibleScore = 1; // Giá trị mặc định để tránh lỗi chia cho 0
                }

                // Tính điểm theo thang 10 hoặc 100 (tùy theo yêu cầu)
                decimal finalScore;
                if (examInfo.Pass_Score <= 10) // Nếu thang điểm là 10
                {
                    finalScore = Math.Round((totalScore / totalPossibleScore) * 10, 2);
                }
                else // Nếu thang điểm là 100
                {
                    finalScore = Math.Round((totalScore / totalPossibleScore) * 100, 2);
                }

                // Kiểm tra đạt/không đạt dựa trên điểm chuẩn
                bool passed = finalScore >= examInfo.Pass_Score;

                await ServiceFactory.UserExamAttempt.UpdateUserExamAttempt(
                    attemptId,
                    DateTime.Now, // Cập nhật thời gian kết thúc
                    finalScore,
                    passed
                );
                
                // Lấy lại thông tin lần làm bài đã cập nhật
                attemptInfo = await ServiceFactory.UserExamAttempt.GetUserExamAttemptById(attemptId);
                
                // 5. Xây dựng kết quả trả về
                int totalQuestions = userAnswers.Count();
                int unansweredQuestions = totalQuestions - (correctAnswers + incorrectAnswers);
                
                // Tính thời gian làm bài
                TimeSpan duration = attemptInfo.End_Time.HasValue 
                    ? attemptInfo.End_Time.Value - attemptInfo.Start_Time 
                    : DateTime.Now - attemptInfo.Start_Time;
                
                // Chuyển đổi danh sách câu trả lời sang dạng chi tiết hơn
                var detailedAnswers = userAnswers.Select(a => new
                {
                    a.Id,
                    a.Question_Id,
                    a.Answer_Given_Json,
                    a.Is_Correct,
                    a.Score_Achieved,
                    a.Time_Spent_Seconds
                }).ToList();
                
                retval.ReturnData = new
                {
                    AttemptId = attemptId,
                    AttemptNumber = attemptNumber,
                    UserId = request.UserId,
                    ExamId = request.ExamId,
                    ExamTitle = examInfo.Title,
                    RawScore = totalScore,
                    TotalPossibleScore = totalPossibleScore,
                    FinalScore = finalScore, // Điểm cuối cùng đã được tính theo thang điểm
                    PassScore = examInfo.Pass_Score,
                    Passed = passed,
                    TotalQuestions = totalQuestions,
                    CorrectAnswers = correctAnswers,
                    IncorrectAnswers = incorrectAnswers,
                    UnansweredQuestions = unansweredQuestions,
                    StartTime = attemptInfo.Start_Time,
                    EndTime = attemptInfo.End_Time ?? DateTime.Now,
                    Duration = $"{duration.Hours:D2}:{duration.Minutes:D2}:{duration.Seconds:D2}",
                    TotalTimeInSeconds = (int)duration.TotalSeconds,
                    DetailedAnswers = detailedAnswers
                };
                
                retval.ReturnStatus.Code = 1;
                retval.ReturnStatus.Message = "Chấm điểm và lấy kết quả chi tiết thành công";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi chấm điểm bài thi");
                retval.ReturnStatus.Message = "Lỗi xử lý: " + ex.Message;
            }
            
            return Ok(retval);
        }

        /// <summary>
        /// Lấy lịch sử làm bài của người dùng với thông tin chi tiết
        /// </summary>
        [HttpGet("History")]
        public async Task<IActionResult> GetUserExamHistory([FromQuery] UserExamHistoryRequest request)
        {
            var retval = new ReturnBaseInfo<object>();
            retval.ReturnStatus = new StatusBaseInfo { Message = "Thất bại", Code = 0 };
            
            if (request == null || request.UserId <= 0)
            {
                retval.ReturnStatus.Message = "Dữ liệu không hợp lệ";
                return Ok(retval);
            }
            
            try
            {
                // Danh sách kết quả sẽ trả về
                var historyResults = new List<UserExamHistoryItem>();
                
                // 1. Lấy danh sách bài thi (nếu examId được chỉ định thì chỉ lấy bài thi đó)
                IEnumerable<ExamInfo> exams;
                if (request.ExamId.HasValue && request.ExamId.Value > 0)
                {
                    var exam = await ServiceFactory.Exam.GetExamById(request.ExamId.Value);
                    exams = exam != null ? new List<ExamInfo> { exam } : new List<ExamInfo>();
                }
                else
                {
                    exams = await ServiceFactory.Exam.GetAllExams();
                }
                
                // 2. Với mỗi bài thi, lấy lịch sử làm bài của người dùng
                foreach (var exam in exams)
                {
                    var attempts = await ServiceFactory.UserExamAttempt.GetUserExamAttemptsByUserIdAndExamId(
                        request.UserId, 
                        exam.Id
                    );
                    
                    if (attempts != null && attempts.Any())
                    {
                        // Nếu có giới hạn số lượng lần làm bài gần nhất
                        if (request.LimitRecentAttempts.HasValue)
                        {
                            attempts = attempts.OrderByDescending(a => a.Attempt_Number)
                                              .Take(request.LimitRecentAttempts.Value)
                                              .ToList();
                        }
                        
                        // 3. Với mỗi lần làm bài, tạo thông tin chi tiết
                        foreach (var attempt in attempts)
                        {
                            // Tính thời gian làm bài
                            TimeSpan duration = attempt.End_Time.HasValue 
                                ? attempt.End_Time.Value - attempt.Start_Time 
                                : TimeSpan.Zero;
                            
                            // Thêm kết quả vào danh sách
                            historyResults.Add(new UserExamHistoryItem
                            {
                                AttemptId = attempt.Id,
                                AttemptNumber = attempt.Attempt_Number,
                                ExamId = exam.Id,
                                ExamTitle = exam.Title,
                                TotalQuestions = exam.Total_Questions,
                                Score = attempt.Total_Score,
                                PassScore = exam.Pass_Score,
                                Passed = attempt.Passed,
                                StartTime = attempt.Start_Time,
                                EndTime = attempt.End_Time,
                                DurationMinutes = (int)duration.TotalMinutes,
                                DurationFormatted = $"{duration.Hours:D2}:{duration.Minutes:D2}:{duration.Seconds:D2}",
                                CompletionStatus = attempt.End_Time.HasValue ? "Hoàn thành" : "Chưa hoàn thành"
                            });
                        }
                    }
                }
                
                // 4. Sắp xếp kết quả theo thời gian bắt đầu (mới nhất lên đầu)
                var sortedResults = historyResults.OrderByDescending(r => r.StartTime).ToList();
                
                // 5. Thống kê tổng hợp
                var summary = new
                {
                    TotalAttempts = sortedResults.Count,
                    TotalExams = sortedResults.Select(r => r.ExamId).Distinct().Count(),
                    TotalPassed = sortedResults.Count(r => r.Passed),
                    TotalFailed = sortedResults.Count(r => !r.Passed),
                    AverageScore = sortedResults.Any() ? sortedResults.Average(r => r.Score) : 0,
                    LastAttemptDate = sortedResults.Any() ? sortedResults.First().StartTime : (DateTime?)null
                };
                
                // 6. Trả về kết quả
                retval.ReturnData = new
                {
                    Summary = summary,
                    History = sortedResults
                };
                
                retval.ReturnStatus.Code = 1;
                retval.ReturnStatus.Message = "Lấy lịch sử làm bài thành công";
                
                return Ok(retval);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy lịch sử làm bài");
                retval.ReturnStatus.Message = "Lỗi xử lý: " + ex.Message;
                return Ok(retval);
            }
        }
        
        /// <summary>
        /// Lấy thông tin tiến độ điểm số của người dùng theo thời gian
        /// </summary>
        [HttpGet("ScoreProgress")]
        public async Task<IActionResult> GetUserScoreProgress([FromQuery] ScoreProgressRequest request)
        {
            var retval = new ReturnBaseInfo<object>();
            retval.ReturnStatus = new StatusBaseInfo { Message = "Thất bại", Code = 0 };
            
            if (request == null || request.UserId <= 0)
            {
                retval.ReturnStatus.Message = "Dữ liệu không hợp lệ";
                return Ok(retval);
            }
            
            try
            {
                // Danh sách kết quả theo thời gian
                var progressResults = new List<object>();
                
                // Lấy danh sách bài thi (tất cả hoặc chỉ bài thi cụ thể)
                IEnumerable<ExamInfo> exams;
                if (request.ExamId.HasValue && request.ExamId.Value > 0)
                {
                    var exam = await ServiceFactory.Exam.GetExamById(request.ExamId.Value);
                    exams = exam != null ? new List<ExamInfo> { exam } : new List<ExamInfo>();
                }
                else
                {
                    exams = await ServiceFactory.Exam.GetAllExams();
                }
                
                // Với mỗi bài thi, lấy lịch sử làm bài
                foreach (var exam in exams)
                {
                    var attempts = await ServiceFactory.UserExamAttempt.GetUserExamAttemptsByUserIdAndExamId(
                        request.UserId, 
                        exam.Id
                    );
                    
                    if (attempts != null && attempts.Any())
                    {
                        // Lọc theo khoảng thời gian nếu có yêu cầu
                        var filteredAttempts = attempts;
                        if (request.FromDate.HasValue)
                        {
                            filteredAttempts = filteredAttempts.Where(a => a.Start_Time >= request.FromDate.Value).ToList();
                        }
                        if (request.ToDate.HasValue)
                        {
                            filteredAttempts = filteredAttempts.Where(a => a.Start_Time <= request.ToDate.Value).ToList();
                        }
                        
                        // Sắp xếp theo thời gian
                        var orderedAttempts = filteredAttempts.OrderBy(a => a.Start_Time).ToList();
                        
                        // Tính tiến độ điểm số theo thời gian
                        foreach (var attempt in orderedAttempts)
                        {
                            if (attempt.End_Time.HasValue && attempt.Total_Score >= 0)
                            {
                                progressResults.Add(new
                                {
                                    ExamId = exam.Id,
                                    ExamTitle = exam.Title,
                                    AttemptId = attempt.Id,
                                    AttemptNumber = attempt.Attempt_Number,
                                    Date = attempt.Start_Time.ToString("yyyy-MM-dd"),
                                    Time = attempt.Start_Time.ToString("HH:mm:ss"),
                                    Score = attempt.Total_Score,
                                    PassScore = exam.Pass_Score,
                                    Passed = attempt.Passed
                                });
                            }
                        }
                    }
                }
                
                // Tính các số liệu tiến độ
                if (progressResults.Any())
                {
                    // Nhóm theo ngày nếu cần
                    var groupedByTime = progressResults;
                    if (request.GroupByDay)
                    {
                        // Nhóm kết quả theo ngày và tính điểm trung bình
                        var groupedResults = progressResults
                            .Cast<dynamic>()
                            .GroupBy(r => r.Date)
                            .Select(g => new
                            {
                                Date = g.Key,
                                AverageScore = g.Average(r => (decimal)r.Score),
                                AttemptCount = g.Count(),
                                PassedCount = g.Count(r => r.Passed == true)
                            })
                            .OrderBy(r => r.Date)
                            .ToList();
                        
                        groupedByTime = groupedResults.Cast<object>().ToList();
                    }
                    
                    // Tính xu hướng tiến bộ
                    decimal firstScore = 0;
                    decimal lastScore = 0;
                    decimal highestScore = 0;
                    decimal lowestScore = 100;
                    
                    if (progressResults.Count > 0)
                    {
                        var firstAttempt = (dynamic)progressResults.First();
                        var lastAttempt = (dynamic)progressResults.Last();
                        firstScore = (decimal)firstAttempt.Score;
                        lastScore = (decimal)lastAttempt.Score;
                        
                        foreach (dynamic result in progressResults)
                        {
                            decimal score = (decimal)result.Score;
                            if (score > highestScore) highestScore = score;
                            if (score < lowestScore) lowestScore = score;
                        }
                    }
                    
                    decimal improvement = progressResults.Count > 1 ? lastScore - firstScore : 0;
                    
                    retval.ReturnData = new
                    {
                        ProgressData = groupedByTime,
                        Summary = new
                        {
                            AttemptCount = progressResults.Count,
                            FirstScore = firstScore,
                            LastScore = lastScore,
                            HighestScore = highestScore,
                            LowestScore = lowestScore,
                            Improvement = improvement,
                            ImprovementPercentage = firstScore > 0 ? Math.Round((improvement / firstScore) * 100, 2) : 0
                        }
                    };
                    
                    retval.ReturnStatus.Code = 1;
                    retval.ReturnStatus.Message = "Lấy thông tin tiến độ điểm số thành công";
                }
                else
                {
                    retval.ReturnData = new { ProgressData = new List<object>(), Summary = new { } };
                    retval.ReturnStatus.Code = 1;
                    retval.ReturnStatus.Message = "Không có dữ liệu tiến độ điểm số trong khoảng thời gian";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy tiến độ điểm số");
                retval.ReturnStatus.Message = "Lỗi xử lý: " + ex.Message;
            }
            
            return Ok(retval);
        }
        
        /// <summary>
        /// Lấy thông tin phân bố điểm số của người dùng hoặc tất cả người dùng
        /// </summary>
        [HttpGet("ScoreDistribution")]
        public async Task<IActionResult> GetScoreDistribution([FromQuery] ScoreDistributionRequest request)
        {
            var retval = new ReturnBaseInfo<object>();
            retval.ReturnStatus = new StatusBaseInfo { Message = "Thất bại", Code = 0 };
            
            try
            {
                // Xác định phạm vi lấy dữ liệu
                int? userId = request.UserId > 0 ? request.UserId : null;
                int? examId = request.ExamId > 0 ? request.ExamId : null;
                
                // Danh sách tất cả lần làm bài
                List<UserExamAttemptInfo> allAttempts = new List<UserExamAttemptInfo>();
                
                if (examId.HasValue)
                {
                    // Nếu chỉ định bài thi cụ thể
                    var exam = await ServiceFactory.Exam.GetExamById(examId.Value);
                    if (exam == null)
                    {
                        retval.ReturnStatus.Message = "Không tìm thấy bài thi";
                        return Ok(retval);
                    }
                    
                    if (userId.HasValue)
                    {
                        // Nếu chỉ định cả người dùng và bài thi
                        var attempts = await ServiceFactory.UserExamAttempt.GetUserExamAttemptsByUserIdAndExamId(
                            userId.Value, 
                            examId.Value
                        );
                        
                        if (attempts != null)
                        {
                            allAttempts.AddRange(attempts);
                        }
                    }
                    else
                    {
                        // Nếu chỉ chỉ định bài thi, lấy tất cả người dùng (cần service bổ sung)
                        // Giả sử có một service lấy tất cả lần làm bài của một bài thi
                        // allAttempts = await ServiceFactory.UserExamAttempt.GetAllAttemptsByExamId(examId.Value);
                        
                        // Trong trường hợp không có service sẵn có, trả về thông báo cần thêm tham số
                        retval.ReturnStatus.Message = "Cần chỉ định UserId để lấy phân bố điểm số cho một bài thi cụ thể";
                        return Ok(retval);
                    }
                }
                else if (userId.HasValue)
                {
                    // Nếu chỉ chỉ định người dùng, lấy tất cả bài thi
                    var exams = await ServiceFactory.Exam.GetAllExams();
                    
                    foreach (var exam in exams)
                    {
                        var attempts = await ServiceFactory.UserExamAttempt.GetUserExamAttemptsByUserIdAndExamId(
                            userId.Value,
                            exam.Id
                        );
                        
                        if (attempts != null)
                        {
                            allAttempts.AddRange(attempts);
                        }
                    }
                }
                else
                {
                    // Nếu không chỉ định gì cả, yêu cầu ít nhất một tham số
                    retval.ReturnStatus.Message = "Cần chỉ định ít nhất một trong hai tham số: UserId hoặc ExamId";
                    return Ok(retval);
                }
                
                // Lọc các lần làm bài đã hoàn thành và có điểm
                var completedAttempts = allAttempts
                    .Where(a => a.End_Time.HasValue && a.Total_Score >= 0)
                    .ToList();
                
                if (completedAttempts.Any())
                {
                    // Xác định phạm vi điểm số và khoảng chia
                    decimal minScore = 0;
                    decimal maxScore = request.UsePercentage ? 100 : 10;
                    int numRanges = request.NumRanges > 0 ? request.NumRanges : 10;
                    
                    // Tính độ rộng của mỗi khoảng
                    decimal rangeSize = (maxScore - minScore) / numRanges;
                    
                    // Tạo các khoảng điểm
                    var scoreRanges = new List<ScoreRange>();
                    for (int i = 0; i < numRanges; i++)
                    {
                        decimal rangeStart = minScore + (i * rangeSize);
                        decimal rangeEnd = rangeStart + rangeSize;
                        
                        if (i == numRanges - 1)
                        {
                            // Đảm bảo khoảng cuối cùng bao gồm giá trị maxScore
                            rangeEnd = maxScore;
                        }
                        
                        scoreRanges.Add(new ScoreRange
                        {
                            RangeStart = rangeStart,
                            RangeEnd = rangeEnd,
                            RangeLabel = $"{rangeStart:0.#}-{rangeEnd:0.#}",
                            Count = 0
                        });
                    }
                    
                    // Đếm số lượng lần làm bài trong mỗi khoảng điểm
                    foreach (var attempt in completedAttempts)
                    {
                        decimal score = attempt.Total_Score;
                        
                        // Tìm khoảng điểm phù hợp
                        var matchingRange = scoreRanges.FirstOrDefault(r => 
                            score >= r.RangeStart && (score < r.RangeEnd || (score == r.RangeEnd && r.RangeEnd == maxScore)));
                        
                        if (matchingRange != null)
                        {
                            matchingRange.Count++;
                        }
                    }
                    
                    // Tính thống kê
                    decimal averageScore = completedAttempts.Average(a => a.Total_Score);
                    int passedCount = completedAttempts.Count(a => a.Passed);
                    
                    retval.ReturnData = new
                    {
                        Distribution = scoreRanges,
                        Summary = new
                        {
                            TotalAttempts = completedAttempts.Count,
                            AverageScore = Math.Round(averageScore, 2),
                            PassedCount = passedCount,
                            PassRate = completedAttempts.Count > 0 ? Math.Round(((decimal)passedCount / completedAttempts.Count) * 100, 2) : 0,
                            ExamId = examId,
                            UserId = userId
                        }
                    };
                    
                    retval.ReturnStatus.Code = 1;
                    retval.ReturnStatus.Message = "Lấy phân bố điểm số thành công";
                }
                else
                {
                    retval.ReturnData = new 
                    { 
                        Distribution = new List<ScoreRange>(),
                        Summary = new { TotalAttempts = 0, AverageScore = 0, PassedCount = 0, PassRate = 0, ExamId = examId, UserId = userId }
                    };
                    retval.ReturnStatus.Code = 1;
                    retval.ReturnStatus.Message = "Không có dữ liệu điểm số phù hợp";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy phân bố điểm số");
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
    
    /// <summary>
    /// Yêu cầu cho API tiến độ điểm số
    /// </summary>
    public class ScoreProgressRequest
    {
        public int UserId { get; set; }
        public int? ExamId { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public bool GroupByDay { get; set; } = false;
    }
    
    /// <summary>
    /// Yêu cầu cho API phân bố điểm số
    /// </summary>
    public class ScoreDistributionRequest
    {
        public int UserId { get; set; }
        public int ExamId { get; set; }
        public int NumRanges { get; set; } = 10;
        public bool UsePercentage { get; set; } = true; // true: thang điểm 100, false: thang điểm 10
    }
    
    /// <summary>
    /// Khoảng điểm số cho phân bố
    /// </summary>
    public class ScoreRange
    {
        public decimal RangeStart { get; set; }
        public decimal RangeEnd { get; set; }
        public string RangeLabel { get; set; }
        public int Count { get; set; }
    }
} 