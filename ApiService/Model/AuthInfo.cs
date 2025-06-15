using ApiService.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiService.Model
{
    public class AuthInfo
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
    public class RefreshTokenInfo
    {
        public string RefrestToken { get; set; }
    }
    public class AuthResponseInfo
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Full_Name { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public bool Is_Admin { get; set; }
    }
}
