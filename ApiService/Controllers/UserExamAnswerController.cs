using ApiService.Business;
using ApiService.Common;
using ApiService.Entity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
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
    }
} 