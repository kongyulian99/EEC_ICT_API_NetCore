using ApiService.Business;
using ApiService.Common;
using ApiService.Entity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ApiService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MailjetController : MyBaseController<MailjetController>
    {
        private readonly IConfiguration _configuration;

        public MailjetController(
            //DI default service
            ILogger<MailjetController> logger,
            AppSetting appSettingInfo,
            //DI services
            IConfiguration configuration
         ) : base(logger, appSettingInfo)
        {
            _configuration = configuration;
            // Thiết lập configuration cho ServiceFactory
            ServiceFactory.SetConfiguration(_configuration);
        }
        
        /// <summary>
        /// Gửi email thử nghiệm
        /// </summary>
        [HttpPost("SendTest")]
        public async Task<IActionResult> SendTestEmail([FromBody] EmailRequest request)
        {
            var retval = new ReturnBaseInfo<bool>();
            retval.ReturnStatus = new StatusBaseInfo { Message = "Thất bại", Code = 0 };
            
            if (request == null || string.IsNullOrEmpty(request.ToEmail))
            {
                retval.ReturnStatus.Message = "Địa chỉ email người nhận không hợp lệ";
                return Ok(retval);
            }
            
            try
            {
                var result = await ServiceFactory.Mailjet.SendEmailAsync(
                    request.ToEmail,
                    "Email thử nghiệm",
                    "<h3>Đây là email thử nghiệm từ hệ thống</h3><p>Email này được gửi để kiểm tra tính năng gửi email qua Mailjet.</p>"
                );
                
                if (result)
                {
                    retval.ReturnData = true;
                    retval.ReturnStatus.Code = 1;
                    retval.ReturnStatus.Message = "Gửi email thành công";
                }
                else
                {
                    retval.ReturnStatus.Message = "Không thể gửi email";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi gửi email thử nghiệm");
                retval.ReturnStatus.Message = "Lỗi: " + ex.Message;
            }
            
            return Ok(retval);
        }
        
        /// <summary>
        /// Gửi email thông báo
        /// </summary>
        [HttpPost("SendNotification")]
        public async Task<IActionResult> SendNotificationEmail([FromBody] NotificationEmailRequest request)
        {
            var retval = new ReturnBaseInfo<bool>();
            retval.ReturnStatus = new StatusBaseInfo { Message = "Thất bại", Code = 0 };
            
            if (request == null || string.IsNullOrEmpty(request.ToEmail) || string.IsNullOrEmpty(request.Subject))
            {
                retval.ReturnStatus.Message = "Thông tin email không hợp lệ";
                return Ok(retval);
            }
            
            try
            {
                // Xây dựng nội dung HTML
                string htmlContent = $@"
                <div style='font-family: Arial, sans-serif; padding: 20px;'>
                    <h2 style='color: #0056b3;'>{request.Subject}</h2>
                    <div style='margin: 20px 0; padding: 15px; background-color: #f8f9fa; border-radius: 5px;'>
                        {request.Message}
                    </div>
                    <p style='color: #6c757d; font-size: 14px;'>Email này được gửi tự động từ hệ thống.</p>
                </div>";
                
                var result = await ServiceFactory.Mailjet.SendEmailAsync(
                    request.ToEmail,
                    request.Subject,
                    htmlContent,
                    request.Message // Plain text version
                );
                
                if (result)
                {
                    retval.ReturnData = true;
                    retval.ReturnStatus.Code = 1;
                    retval.ReturnStatus.Message = "Gửi email thông báo thành công";
                }
                else
                {
                    retval.ReturnStatus.Message = "Không thể gửi email thông báo";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi gửi email thông báo");
                retval.ReturnStatus.Message = "Lỗi: " + ex.Message;
            }
            
            return Ok(retval);
        }
        
        /// <summary>
        /// Gửi email xác nhận đăng ký tài khoản
        /// </summary>
        [HttpPost("SendRegistrationConfirmation")]
        public async Task<IActionResult> SendRegistrationConfirmation([FromBody] RegistrationEmailRequest request)
        {
            var retval = new ReturnBaseInfo<bool>();
            retval.ReturnStatus = new StatusBaseInfo { Message = "Thất bại", Code = 0 };
            
            if (request == null || string.IsNullOrEmpty(request.ToEmail) || string.IsNullOrEmpty(request.Username))
            {
                retval.ReturnStatus.Message = "Thông tin người dùng không hợp lệ";
                return Ok(retval);
            }
            
            try
            {
                // Xây dựng nội dung HTML cho email xác nhận đăng ký
                string htmlContent = $@"
                <div style='font-family: Arial, sans-serif; padding: 20px;'>
                    <h2 style='color: #0056b3;'>Xác nhận đăng ký tài khoản</h2>
                    <p>Chào {request.Username},</p>
                    <p>Cảm ơn bạn đã đăng ký tài khoản trên hệ thống của chúng tôi.</p>
                    <p>Thông tin tài khoản của bạn:</p>
                    <ul>
                        <li>Tên đăng nhập: <strong>{request.Username}</strong></li>
                        <li>Email: <strong>{request.ToEmail}</strong></li>
                    </ul>
                    <p>Vui lòng nhấp vào nút bên dưới để xác nhận email của bạn:</p>
                    <div style='margin: 25px 0;'>
                        <a href='{request.ConfirmationLink}' style='background-color: #0056b3; color: white; padding: 10px 20px; text-decoration: none; border-radius: 5px;'>
                            Xác nhận email
                        </a>
                    </div>
                    <p>Nếu bạn không thể nhấp vào nút, vui lòng sao chép và dán đường dẫn sau vào trình duyệt:</p>
                    <p>{request.ConfirmationLink}</p>
                    <p>Nếu bạn không thực hiện yêu cầu này, vui lòng bỏ qua email này.</p>
                    <p style='color: #6c757d; font-size: 14px; margin-top: 30px;'>Email này được gửi tự động từ hệ thống.</p>
                </div>";
                
                string plainTextContent = $@"
                Xác nhận đăng ký tài khoản
                
                Chào {request.Username},
                
                Cảm ơn bạn đã đăng ký tài khoản trên hệ thống của chúng tôi.
                
                Thông tin tài khoản của bạn:
                - Tên đăng nhập: {request.Username}
                - Email: {request.ToEmail}
                
                Vui lòng truy cập đường dẫn sau để xác nhận email của bạn:
                {request.ConfirmationLink}
                
                Nếu bạn không thực hiện yêu cầu này, vui lòng bỏ qua email này.
                
                Email này được gửi tự động từ hệ thống.";
                
                var result = await ServiceFactory.Mailjet.SendEmailAsync(
                    request.ToEmail,
                    "Xác nhận đăng ký tài khoản",
                    htmlContent,
                    plainTextContent
                );
                
                if (result)
                {
                    retval.ReturnData = true;
                    retval.ReturnStatus.Code = 1;
                    retval.ReturnStatus.Message = "Gửi email xác nhận đăng ký thành công";
                }
                else
                {
                    retval.ReturnStatus.Message = "Không thể gửi email xác nhận đăng ký";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi gửi email xác nhận đăng ký");
                retval.ReturnStatus.Message = "Lỗi: " + ex.Message;
            }
            
            return Ok(retval);
        }
        
        /// <summary>
        /// Gửi email đặt lại mật khẩu
        /// </summary>
        [HttpPost("SendPasswordReset")]
        public async Task<IActionResult> SendPasswordReset([FromBody] PasswordResetEmailRequest request)
        {
            var retval = new ReturnBaseInfo<bool>();
            retval.ReturnStatus = new StatusBaseInfo { Message = "Thất bại", Code = 0 };
            
            if (request == null || string.IsNullOrEmpty(request.ToEmail))
            {
                retval.ReturnStatus.Message = "Địa chỉ email không hợp lệ";
                return Ok(retval);
            }
            
            try
            {
                // Xây dựng nội dung HTML cho email đặt lại mật khẩu
                string htmlContent = $@"
                <div style='font-family: Arial, sans-serif; padding: 20px;'>
                    <h2 style='color: #0056b3;'>Đặt lại mật khẩu</h2>
                    <p>Chào bạn,</p>
                    <p>Chúng tôi nhận được yêu cầu đặt lại mật khẩu cho tài khoản của bạn.</p>
                    <p>Vui lòng nhấp vào nút bên dưới để đặt lại mật khẩu:</p>
                    <div style='margin: 25px 0;'>
                        <a href='{request.ResetLink}' style='background-color: #0056b3; color: white; padding: 10px 20px; text-decoration: none; border-radius: 5px;'>
                            Đặt lại mật khẩu
                        </a>
                    </div>
                    <p>Nếu bạn không thể nhấp vào nút, vui lòng sao chép và dán đường dẫn sau vào trình duyệt:</p>
                    <p>{request.ResetLink}</p>
                    <p>Liên kết này sẽ hết hạn sau {request.ExpirationHours} giờ.</p>
                    <p>Nếu bạn không thực hiện yêu cầu này, vui lòng bỏ qua email này.</p>
                    <p style='color: #6c757d; font-size: 14px; margin-top: 30px;'>Email này được gửi tự động từ hệ thống.</p>
                </div>";
                
                string plainTextContent = $@"
                Đặt lại mật khẩu
                
                Chào bạn,
                
                Chúng tôi nhận được yêu cầu đặt lại mật khẩu cho tài khoản của bạn.
                
                Vui lòng truy cập đường dẫn sau để đặt lại mật khẩu:
                {request.ResetLink}
                
                Liên kết này sẽ hết hạn sau {request.ExpirationHours} giờ.
                
                Nếu bạn không thực hiện yêu cầu này, vui lòng bỏ qua email này.
                
                Email này được gửi tự động từ hệ thống.";
                
                var result = await ServiceFactory.Mailjet.SendEmailAsync(
                    request.ToEmail,
                    "Đặt lại mật khẩu",
                    htmlContent,
                    plainTextContent
                );
                
                if (result)
                {
                    retval.ReturnData = true;
                    retval.ReturnStatus.Code = 1;
                    retval.ReturnStatus.Message = "Gửi email đặt lại mật khẩu thành công";
                }
                else
                {
                    retval.ReturnStatus.Message = "Không thể gửi email đặt lại mật khẩu";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi gửi email đặt lại mật khẩu");
                retval.ReturnStatus.Message = "Lỗi: " + ex.Message;
            }
            
            return Ok(retval);
        }
    }
    
    public class EmailRequest
    {
        public string ToEmail { get; set; }
    }
    
    public class NotificationEmailRequest
    {
        public string ToEmail { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }
    }
    
    public class RegistrationEmailRequest
    {
        public string ToEmail { get; set; }
        public string Username { get; set; }
        public string ConfirmationLink { get; set; }
    }
    
    public class PasswordResetEmailRequest
    {
        public string ToEmail { get; set; }
        public string ResetLink { get; set; }
        public int ExpirationHours { get; set; } = 24;
    }
} 