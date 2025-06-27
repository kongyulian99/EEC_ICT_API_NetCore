using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiService.Entity
{
    /// <summary>
    /// Lớp lưu trữ thông tin đăng nhập bằng Google
    /// </summary>
    public class GoogleLoginRequest
    {
        /// <summary>
        /// JWT token từ Google OAuth (id_token)
        /// </summary>
        public string IdToken { get; set; }
    }
}