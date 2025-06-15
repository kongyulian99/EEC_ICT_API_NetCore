using ApiService.Business;
using ApiService.Common;
using ApiService.Entity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text.Json;
using System.Threading.Tasks;

namespace ApiService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuestionController : MyBaseController<QuestionController>
    {
        private readonly IMemoryCache _memoryCache;

        public QuestionController(
            //DI default service
            ILogger<QuestionController> logger,
            AppSetting appSettingInfo,
            //DI services
            IMemoryCache memoryCache
         ) : base(logger, appSettingInfo)
        {
            _memoryCache = memoryCache;
        }
        
        /// <summary>
        /// Lấy tất cả câu hỏi
        /// </summary>
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAllQuestions()
        {
            var retval = new ReturnBaseInfo<IEnumerable<QuestionInfo>>();
            retval.ReturnStatus = new StatusBaseInfo { Message = "Thất bại", Code = 0 };
            
            var questions = await ServiceFactory.Question.GetAllQuestions();
            if (questions != null)
            {
                retval.ReturnData = questions;
                retval.ReturnStatus.Code = 1;
                retval.ReturnStatus.Message = "Lấy danh sách câu hỏi thành công";
            }
            else
            {
                retval.ReturnStatus.Message = "Không thể lấy danh sách câu hỏi";
            }
            
            return Ok(retval);
        }
        
        /// <summary>
        /// Lấy câu hỏi theo ID
        /// </summary>
        [HttpGet("GetById/{id}")]
        public async Task<IActionResult> GetQuestionById(int id)
        {
            var retval = new ReturnBaseInfo<QuestionInfo>();
            retval.ReturnStatus = new StatusBaseInfo { Message = "Thất bại", Code = 0 };
            
            if (id <= 0)
            {
                retval.ReturnStatus.Message = "ID câu hỏi không hợp lệ";
                return Ok(retval);
            }
            
            var question = await ServiceFactory.Question.GetQuestionById(id);
            if (question != null)
            {
                retval.ReturnData = question;
                retval.ReturnStatus.Code = 1;
                retval.ReturnStatus.Message = "Lấy thông tin câu hỏi thành công";
            }
            else
            {
                retval.ReturnStatus.Message = "Không tìm thấy câu hỏi";
            }
            
            return Ok(retval);
        }
        
        /// <summary>
        /// Lấy câu hỏi theo Topic ID
        /// </summary>
        [HttpGet("GetByTopicId/{topicId}")]
        public async Task<IActionResult> GetQuestionsByTopicId(int topicId)
        {
            var retval = new ReturnBaseInfo<IEnumerable<QuestionInfo>>();
            retval.ReturnStatus = new StatusBaseInfo { Message = "Thất bại", Code = 0 };
            
            if (topicId <= 0)
            {
                retval.ReturnStatus.Message = "ID chủ đề không hợp lệ";
                return Ok(retval);
            }
            
            var questions = await ServiceFactory.Question.GetQuestionsByTopicId(topicId);
            if (questions != null)
            {
                retval.ReturnData = questions;
                retval.ReturnStatus.Code = 1;
                retval.ReturnStatus.Message = "Lấy danh sách câu hỏi theo chủ đề thành công";
            }
            else
            {
                retval.ReturnStatus.Message = "Không thể lấy danh sách câu hỏi theo chủ đề";
            }
            
            return Ok(retval);
        }
        
        /// <summary>
        /// Lấy câu hỏi theo Exam ID
        /// </summary>
        [HttpGet("GetByExamId/{examId}")]
        public async Task<IActionResult> GetQuestionsByExamId(int examId)
        {
            var retval = new ReturnBaseInfo<IEnumerable<QuestionInfo>>();
            retval.ReturnStatus = new StatusBaseInfo { Message = "Thất bại", Code = 0 };
            
            if (examId <= 0)
            {
                retval.ReturnStatus.Message = "ID đề thi không hợp lệ";
                return Ok(retval);
            }
            
            var questions = await ServiceFactory.Question.GetQuestionsByExamId(examId);
            if (questions != null)
            {
                retval.ReturnData = questions;
                retval.ReturnStatus.Code = 1;
                retval.ReturnStatus.Message = "Lấy danh sách câu hỏi theo đề thi thành công";
            }
            else
            {
                retval.ReturnStatus.Message = "Không thể lấy danh sách câu hỏi theo đề thi";
            }
            
            return Ok(retval);
        }
        
        /// <summary>
        /// Lấy câu hỏi theo mức độ khó
        /// </summary>
        [HttpGet("GetByDifficultyLevel/{difficultyLevel}")]
        public async Task<IActionResult> GetQuestionsByDifficultyLevel(Enum_DifficutyLevel difficultyLevel)
        {
            var retval = new ReturnBaseInfo<IEnumerable<QuestionInfo>>();
            retval.ReturnStatus = new StatusBaseInfo { Message = "Thất bại", Code = 0 };
            
            var questions = await ServiceFactory.Question.GetQuestionsByDifficultyLevel(difficultyLevel);
            if (questions != null)
            {
                retval.ReturnData = questions;
                retval.ReturnStatus.Code = 1;
                retval.ReturnStatus.Message = "Lấy danh sách câu hỏi theo mức độ khó thành công";
            }
            else
            {
                retval.ReturnStatus.Message = "Không thể lấy danh sách câu hỏi theo mức độ khó";
            }
            
            return Ok(retval);
        }
        
        /// <summary>
        /// Tạo câu hỏi mới
        /// </summary>
        [HttpPost("Create")]
        public async Task<IActionResult> CreateQuestion([FromBody] QuestionInfo question)
        {
            var retval = new ReturnBaseInfo<int>();
            retval.ReturnStatus = new StatusBaseInfo { Message = "Thất bại", Code = 0 };
            
            if (question == null || string.IsNullOrEmpty(question.Content) || question.Topic_Id <= 0 || question.Exam_Id <= 0 || question.Question_Data_Json == null)
            {
                retval.ReturnStatus.Message = "Dữ liệu không hợp lệ";
                return Ok(retval);
            }
            
            // Đặt giá trị mặc định cho Difficulty_Level nếu không được cung cấp
            if (question.Difficulty_Level == 0)
            {
                question.Difficulty_Level = Enum_DifficutyLevel.MEDIUM;
            }
            
            var questionId = await ServiceFactory.Question.CreateQuestion(
                question.Topic_Id, 
                question.Exam_Id, 
                question.Question_Type, 
                question.Content, 
                question.Question_Data_Json,
                question.Explanation, 
                question.Difficulty_Level);
                
            if (questionId > 0)
            {
                retval.ReturnData = questionId;
                retval.ReturnStatus.Code = 1;
                retval.ReturnStatus.Message = "Tạo câu hỏi thành công";
            }
            else
            {
                retval.ReturnStatus.Message = "Không thể tạo câu hỏi";
            }
            
            return Ok(retval);
        }
        
        /// <summary>
        /// Cập nhật thông tin câu hỏi
        /// </summary>
        [HttpPut("Update")]
        public async Task<IActionResult> UpdateQuestion([FromBody] QuestionInfo question)
        {
            var retval = new ReturnBaseInfo<bool>();
            retval.ReturnStatus = new StatusBaseInfo { Message = "Thất bại", Code = 0 };
            
            if (question == null || question.Id <= 0 || string.IsNullOrEmpty(question.Content) || 
                question.Topic_Id <= 0 || question.Exam_Id <= 0 || question.Question_Data_Json == null)
            {
                retval.ReturnStatus.Message = "Dữ liệu không hợp lệ";
                return Ok(retval);
            }
            
            // Đặt giá trị mặc định cho Difficulty_Level nếu không được cung cấp
            if (question.Difficulty_Level == 0)
            {
                question.Difficulty_Level = Enum_DifficutyLevel.MEDIUM;
            }
            
            var result = await ServiceFactory.Question.UpdateQuestion(
                question.Id,
                question.Topic_Id, 
                question.Exam_Id, 
                question.Question_Type, 
                question.Content, 
                question.Question_Data_Json,
                question.Explanation, 
                question.Difficulty_Level);
                
            if (result)
            {
                retval.ReturnData = true;
                retval.ReturnStatus.Code = 1;
                retval.ReturnStatus.Message = "Cập nhật câu hỏi thành công";
            }
            else
            {
                retval.ReturnStatus.Message = "Không thể cập nhật câu hỏi";
            }
            
            return Ok(retval);
        }
        
        /// <summary>
        /// Xóa câu hỏi
        /// </summary>
        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> DeleteQuestion(int id)
        {
            var retval = new ReturnBaseInfo<bool>();
            retval.ReturnStatus = new StatusBaseInfo { Message = "Thất bại", Code = 0 };
            
            if (id <= 0)
            {
                retval.ReturnStatus.Message = "ID câu hỏi không hợp lệ";
                return Ok(retval);
            }
            
            var result = await ServiceFactory.Question.DeleteQuestion(id);
            if (result)
            {
                retval.ReturnData = true;
                retval.ReturnStatus.Code = 1;
                retval.ReturnStatus.Message = "Xóa câu hỏi thành công";
            }
            else
            {
                retval.ReturnStatus.Message = "Không thể xóa câu hỏi";
            }
            
            return Ok(retval);
        }
    }
} 