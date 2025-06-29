using ApiService.Business;
using ApiService.Common;
using ApiService.Entity;
using ApiService.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController : MyBaseController<DashboardController>
    {
        public DashboardController(ILogger<DashboardController> logger, AppSetting appSettingInfo)
            : base(logger, appSettingInfo)
        {
        }
        
        /// <summary>
        /// Lấy thông tin tổng quan của hệ thống: số lượng người dùng, exam, question, attempts, pass rate
        /// </summary>
        [HttpGet("SystemSummary")]
        public IActionResult GetSystemSummary()
        {
            var retval = new ReturnBaseInfo<DashboardSummary>();
            retval.ReturnStatus = new StatusBaseInfo { Message = "Thất bại", Code = 0 };
            
            try
            {
                var result = ServiceFactory.Dashboard.GetSystemSummary();
                if (result.Code > 0)
                {
                    retval.ReturnData = result.Result;
                    retval.ReturnStatus.Code = 1;
                    retval.ReturnStatus.Message = "Lấy thông tin tổng quan thành công";
                }
                else
                {
                    retval.ReturnStatus.Message = result.Message;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy thông tin tổng quan");
                retval.ReturnStatus.Message = "Lỗi hệ thống: " + ex.Message;
            }
            
            return Ok(retval);
        }
        
        /// <summary>
        /// Lấy dữ liệu về số lượng attempts theo thời gian
        /// </summary>
        [HttpGet("ExamAttemptsOverTime")]
        public IActionResult GetExamAttemptsOverTime([FromQuery] DateTime? fromDate, [FromQuery] DateTime? toDate)
        {
            var retval = new ReturnBaseInfo<List<ExamAttemptsOverTime>>();
            retval.ReturnStatus = new StatusBaseInfo { Message = "Thất bại", Code = 0 };
            
            try
            {
                var result = ServiceFactory.Dashboard.GetExamAttemptsOverTime(fromDate, toDate);
                if (result.Code > 0)
                {
                    retval.ReturnData = result.Result;
                    retval.ReturnStatus.Code = 1;
                    retval.ReturnStatus.Message = "Lấy dữ liệu thành công";
                }
                else
                {
                    retval.ReturnStatus.Message = result.Message;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy dữ liệu attempts theo thời gian");
                retval.ReturnStatus.Message = "Lỗi hệ thống: " + ex.Message;
            }
            
            return Ok(retval);
        }
        
        /// <summary>
        /// Lấy phân bố điểm của các lần làm bài thi (thang điểm 10, chia thành 5 khoảng)
        /// </summary>
        [HttpGet("ScoreDistribution")]
        public IActionResult GetScoreDistribution([FromQuery] int? examId)
        {
            var retval = new ReturnBaseInfo<List<ScoreDistribution>>();
            retval.ReturnStatus = new StatusBaseInfo { Message = "Thất bại", Code = 0 };
            
            try
            {
                var result = ServiceFactory.Dashboard.GetScoreDistribution(examId);
                if (result.Code > 0)
                {
                    retval.ReturnData = result.Result;
                    retval.ReturnStatus.Code = 1;
                    retval.ReturnStatus.Message = "Lấy phân bố điểm thành công";
                }
                else
                {
                    retval.ReturnStatus.Message = result.Message;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy phân bố điểm");
                retval.ReturnStatus.Message = "Lỗi hệ thống: " + ex.Message;
            }
            
            return Ok(retval);
        }
        
        /// <summary>
        /// Lấy danh sách các bài thi có tỷ lệ đậu cao nhất
        /// </summary>
        [HttpGet("TopExamsByPassRate")]
        public IActionResult GetTopExamsByPassRate([FromQuery] int limit = 10)
        {
            var retval = new ReturnBaseInfo<List<TopExamByPassRate>>();
            retval.ReturnStatus = new StatusBaseInfo { Message = "Thất bại", Code = 0 };
            
            try
            {
                var result = ServiceFactory.Dashboard.GetTopExamsByPassRate(limit);
                if (result.Code > 0)
                {
                    retval.ReturnData = result.Result;
                    retval.ReturnStatus.Code = 1;
                    retval.ReturnStatus.Message = "Lấy danh sách bài thi thành công";
                }
                else
                {
                    retval.ReturnStatus.Message = result.Message;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy danh sách bài thi theo tỷ lệ đậu");
                retval.ReturnStatus.Message = "Lỗi hệ thống: " + ex.Message;
            }
            
            return Ok(retval);
        }
        
        /// <summary>
        /// Lấy các hoạt động gần đây trong hệ thống, bao gồm tên bài thi và điểm số
        /// </summary>
        [HttpGet("RecentActivities")]
        public IActionResult GetRecentActivities([FromQuery] int limit = 20)
        {
            var retval = new ReturnBaseInfo<List<RecentActivity>>();
            retval.ReturnStatus = new StatusBaseInfo { Message = "Thất bại", Code = 0 };
            
            try
            {
                var result = ServiceFactory.Dashboard.GetRecentActivities(limit);
                if (result.Code > 0)
                {
                    retval.ReturnData = result.Result;
                    retval.ReturnStatus.Code = 1;
                    retval.ReturnStatus.Message = "Lấy hoạt động gần đây thành công";
                }
                else
                {
                    retval.ReturnStatus.Message = result.Message;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy hoạt động gần đây");
                retval.ReturnStatus.Message = "Lỗi hệ thống: " + ex.Message;
            }
            
            return Ok(retval);
        }
    }
} 