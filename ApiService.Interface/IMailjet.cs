using ApiService.Entity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace ApiService.Interface
{
    public interface IMailjet
    {
        /// <summary>
        /// Gửi email thông qua Mailjet API
        /// </summary>
        /// <param name="toEmail">Địa chỉ email người nhận</param>
        /// <param name="subject">Tiêu đề email</param>
        /// <param name="htmlContent">Nội dung HTML</param>
        /// <param name="plainTextContent">Nội dung text (không bắt buộc)</param>
        /// <returns>Kết quả gửi email: true nếu thành công, false nếu thất bại</returns>
        Task<bool> SendEmailAsync(string toEmail, string subject, string htmlContent, string plainTextContent = null);
    }
} 