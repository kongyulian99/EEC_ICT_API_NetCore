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
    public class ExamController : MyBaseController<ExamController>
    {
        private readonly IMemoryCache _memoryCache;

        public ExamController(
            //DI default service
            ILogger<ExamController> logger,
            AppSetting appSettingInfo,
            //DI services
            IMemoryCache memoryCache
         ) : base(logger, appSettingInfo)
        {
            _memoryCache = memoryCache;
        }
        
        /// <summary>
        /// Lấy tất cả đề thi
        /// </summary>
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAllExams()
        {
            var retval = new ReturnBaseInfo<IEnumerable<ExamInfo>>();
            retval.ReturnStatus = new StatusBaseInfo { Message = "Thất bại", Code = 0 };
            
            var exams = await ServiceFactory.Exam.GetAllExams();
            if (exams != null)
            {
                retval.ReturnData = exams;
                retval.ReturnStatus.Code = 1;
                retval.ReturnStatus.Message = "Lấy danh sách đề thi thành công";
            }
            else
            {
                retval.ReturnStatus.Message = "Không thể lấy danh sách đề thi";
            }
            
            return Ok(retval);
        }
        
        /// <summary>
        /// Lấy đề thi theo ID
        /// </summary>
        [HttpGet("GetById/{id}")]
        public async Task<IActionResult> GetExamById(int id)
        {
            var retval = new ReturnBaseInfo<ExamInfo>();
            retval.ReturnStatus = new StatusBaseInfo { Message = "Thất bại", Code = 0 };
            
            if (id <= 0)
            {
                retval.ReturnStatus.Message = "ID đề thi không hợp lệ";
                return Ok(retval);
            }
            
            var exam = await ServiceFactory.Exam.GetExamById(id);
            if (exam != null)
            {
                retval.ReturnData = exam;
                retval.ReturnStatus.Code = 1;
                retval.ReturnStatus.Message = "Lấy thông tin đề thi thành công";
            }
            else
            {
                retval.ReturnStatus.Message = "Không tìm thấy đề thi";
            }
            
            return Ok(retval);
        }
        
        /// <summary>
        /// Tạo đề thi mới
        /// </summary>
        [HttpPost("Create")]
        public async Task<IActionResult> CreateExam([FromBody] ExamInfo exam)
        {
            var retval = new ReturnBaseInfo<int>();
            retval.ReturnStatus = new StatusBaseInfo { Message = "Thất bại", Code = 0 };
            
            if (exam == null || string.IsNullOrEmpty(exam.Title))
            {
                retval.ReturnStatus.Message = "Dữ liệu không hợp lệ";
                return Ok(retval);
            }
            
            var examId = await ServiceFactory.Exam.CreateExam(exam);
            if (examId > 0)
            {
                retval.ReturnData = examId;
                retval.ReturnStatus.Code = 1;
                retval.ReturnStatus.Message = "Tạo đề thi thành công";
            }
            else
            {
                retval.ReturnStatus.Message = "Không thể tạo đề thi";
            }
            
            return Ok(retval);
        }
        
        /// <summary>
        /// Cập nhật thông tin đề thi
        /// </summary>
        [HttpPut("Update")]
        public async Task<IActionResult> UpdateExam([FromBody] ExamInfo exam)
        {
            var retval = new ReturnBaseInfo<bool>();
            retval.ReturnStatus = new StatusBaseInfo { Message = "Thất bại", Code = 0 };
            
            if (exam == null || exam.Id <= 0 || string.IsNullOrEmpty(exam.Title))
            {
                retval.ReturnStatus.Message = "Dữ liệu không hợp lệ";
                return Ok(retval);
            }
            
            var result = await ServiceFactory.Exam.UpdateExam(exam);
            if (result)
            {
                retval.ReturnData = true;
                retval.ReturnStatus.Code = 1;
                retval.ReturnStatus.Message = "Cập nhật đề thi thành công";
            }
            else
            {
                retval.ReturnStatus.Message = "Không thể cập nhật đề thi";
            }
            
            return Ok(retval);
        }
        
        /// <summary>
        /// Xóa đề thi
        /// </summary>
        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> DeleteExam(int id)
        {
            var retval = new ReturnBaseInfo<bool>();
            retval.ReturnStatus = new StatusBaseInfo { Message = "Thất bại", Code = 0 };
            
            if (id <= 0)
            {
                retval.ReturnStatus.Message = "ID đề thi không hợp lệ";
                return Ok(retval);
            }
            
            var result = await ServiceFactory.Exam.DeleteExam(id);
            if (result)
            {
                retval.ReturnData = true;
                retval.ReturnStatus.Code = 1;
                retval.ReturnStatus.Message = "Xóa đề thi thành công";
            }
            else
            {
                retval.ReturnStatus.Message = "Không thể xóa đề thi";
            }
            
            return Ok(retval);
        }
    }
} 