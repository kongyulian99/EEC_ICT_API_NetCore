using ApiService.Business;
using ApiService.Common;
using ApiService.Core;
using ApiService.Entity;
using ApiService.Model;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ApiService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ILogger<AuthController> _logger;
        private AppSetting _appSettingInfo;
        private AuthService _authSv;      
        private readonly IMemoryCache _memoryCache;
        private readonly IConfiguration _configuration;

        public AuthController(
            ILogger<AuthController> logger
            , AppSetting appSettingInfo
            , AuthService authSv
            , IMemoryCache memoryCache
            , IConfiguration configuration)
        {
            _logger = logger;
            _appSettingInfo = appSettingInfo;
            _authSv = authSv;
            _memoryCache = memoryCache;
            _configuration = configuration;
        }

        [HttpPost("Login")]
        public IActionResult Login(AuthInfo input)
        {
            var retval = new ReturnBaseInfo<AuthResponseInfo>();
            retval.ReturnStatus = new StatusBaseInfo { Message = "Không thành công", Code = 0 };
            retval.ReturnData = null;
            if (string.IsNullOrEmpty(input.Username) || string.IsNullOrEmpty(input.Password))
            {
                retval.ReturnStatus.Code = -1;
                retval.ReturnStatus.Message = "Truyền thiếu thông tin";
                return Ok(retval);
            }
            //login
            var checkLogin = _authSv.CheckLogin(input.Username, input.Password);
            if (checkLogin.ReturnStatus.Code != 1)
            {
                retval.ReturnStatus.Code = -1;
                retval.ReturnStatus.Message = checkLogin.ReturnStatus.Message;
                return Ok(retval);
            }
            
            //get User info
            var userInfo = ServiceFactory.User.GetUserByUsername(input.Username);
            if (userInfo.Code != 1 || userInfo.Result == null)
            {
                retval.ReturnStatus.Code = -1;
                retval.ReturnStatus.Message = "Không tìm thấy thông tin người dùng";
                return Ok(retval);
            }

            var token = "";
            var refreshToken = "";
            var forceLogoutKey = Guid.NewGuid().ToString();
            setNewToken(userInfo.Result, forceLogoutKey, out token, out refreshToken);

            var objectData = new AuthResponseInfo();
            objectData.AccessToken = token;
            objectData.Id = userInfo.Result.Id;
            objectData.RefreshToken = refreshToken;
            objectData.Is_Admin = userInfo.Result.Is_Admin;
            objectData.Username = userInfo.Result.Username;
            objectData.Full_Name = userInfo.Result.Full_Name;

            retval.ReturnStatus.Message = "Thành công";
            retval.ReturnStatus.Code = 1;
            retval.ReturnData = objectData;
            return Ok(retval);
        }
        
        [HttpPost("RefreshToken")]
        public IActionResult RefreshToken(RefreshTokenInfo input)
        {
            var retval = new ReturnBaseInfo<AuthResponseInfo>();
            retval.ReturnStatus = new StatusBaseInfo { Message = "Không thành công", Code = 0 };
            retval.ReturnData = null;
            if (string.IsNullOrEmpty(input.RefrestToken))
            {
                retval.ReturnStatus.Message = "Truyền thiếu thông tin";
                return Ok(retval);
            }
            if (!VerifyToken(input.RefrestToken.Replace("bearer ", "")))
            {
                retval.ReturnStatus.Message = "RefrestToken hết hạn, cần login lại!";
                retval.ReturnStatus.Code = -999;
                return Ok(retval);
            }
            var handler = new JwtSecurityTokenHandler();
            var jwtSecurityToken = handler.ReadJwtToken(input.RefrestToken.Replace("bearer ", ""));
            var sessionKey ="";
            try
            {
                sessionKey = jwtSecurityToken.Claims.First(claim => claim.Type == "SessionKey").Value;
            }
            catch { }
            
            if (string.IsNullOrEmpty(sessionKey))
            {
                retval.ReturnStatus.Message = "Refreshtoken không đúng!";
                return Ok(retval);
            }

            var userName = jwtSecurityToken.Claims.First(claim => claim.Type == "UserName").Value;
            var forceLogoutKeyToken = jwtSecurityToken.Claims.First(claim => claim.Type == "ForceLogoutKey").Value;
            
            // Lấy thông tin người dùng từ database
            var userInfo = ServiceFactory.User.GetUserByUsername(userName);
            if (userInfo.Code != 1 || userInfo.Result == null)
            {
                retval.ReturnStatus.Message = "Không tìm thấy thông tin người dùng";
                return Ok(retval);
            }
            
            // Kiểm tra trạng thái người dùng
            if (!userInfo.Result.Is_Active)
            {
                retval.ReturnStatus.Message = "Tài khoản đã bị khóa hoặc không hoạt động";
                return Ok(retval);
            }

            var token = "";
            var refreshToken = "";
            var newForceLogoutKey = Guid.NewGuid().ToString();
            setNewToken(userInfo.Result, newForceLogoutKey, out token, out refreshToken);

            var objectData = new AuthResponseInfo();
            objectData.AccessToken = token;
            objectData.Id = userInfo.Result.Id;
            objectData.RefreshToken = refreshToken;
            objectData.Is_Admin = userInfo.Result.Is_Admin;
            objectData.Username = userInfo.Result.Username;
            objectData.Full_Name = userInfo.Result.Full_Name;

            retval.ReturnStatus.Message = "Thành công";
            retval.ReturnStatus.Code = 1;
            retval.ReturnData = objectData;
            return Ok(retval);
        }
        
        private void setNewToken(UserInfo userInfo, string forceLogoutKey, out string token, out string refreshToken)
        {
            token = "";
            refreshToken = "";

            // Cập nhật thời gian đăng nhập
            ServiceFactory.User.UpdateLastLogin(userInfo.Id);

            // Lưu thông tin người dùng vào cache
            var cacheUserInfoKey = MyConstant.UserInfoCacheKey + userInfo.Username;
            _memoryCache.Remove(cacheUserInfoKey);
            _memoryCache.Set(cacheUserInfoKey, userInfo, new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromMinutes(_appSettingInfo.Jwt.TokenLifeTime + _appSettingInfo.Jwt.RefreshTokenAppendTime)));

            //Tạo Token JWT
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_appSettingInfo.Jwt.Key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var claims = new[]
            {
                new Claim("UserName", userInfo.Username),
                new Claim("ForceLogoutKey", forceLogoutKey),
                new Claim("UserId", userInfo.Id.ToString()),
                new Claim("IsAdmin", userInfo.Is_Admin.ToString())
            };
            var sectoken = new JwtSecurityToken(_appSettingInfo.Jwt.Issuer,
              _appSettingInfo.Jwt.Issuer,
              claims,
              expires: DateTime.Now.AddMinutes(_appSettingInfo.Jwt.TokenLifeTime),
              signingCredentials: credentials);
            token = new JwtSecurityTokenHandler().WriteToken(sectoken);
            token = "bearer " + token;

            var secRefreshttoken = new JwtSecurityToken(_appSettingInfo.Jwt.Issuer,
              _appSettingInfo.Jwt.Issuer,
              new[]
                {
                    new Claim("SessionKey", Guid.NewGuid().ToString()),
                    new Claim("UserName", userInfo.Username),
                    new Claim("ForceLogoutKey", forceLogoutKey),
                    new Claim("UserId", userInfo.Id.ToString()),
                    new Claim("IsAdmin", userInfo.Is_Admin.ToString())
                },
              expires: DateTime.Now.AddMinutes(_appSettingInfo.Jwt.TokenLifeTime + _appSettingInfo.Jwt.RefreshTokenAppendTime),
              signingCredentials: credentials);
            refreshToken = new JwtSecurityTokenHandler().WriteToken(secRefreshttoken);
            refreshToken = "bearer " + refreshToken;
        }
        
        private bool VerifyToken(string token)
        {
            var validationParameters = new TokenValidationParameters()
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = _appSettingInfo.Jwt.Issuer,
                ValidAudience = _appSettingInfo.Jwt.Issuer,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_appSettingInfo.Jwt.Key))
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken validatedToken = null;
            try
            {
                tokenHandler.ValidateToken(token, validationParameters, out validatedToken);
            }
            catch (SecurityTokenException)
            {
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + System.Environment.NewLine + ex.StackTrace);
                return false;
            }
            return validatedToken != null;
        }

        private string GetIpAddress()
        {
            var sIpAddress = "";
            try
            {
                if (Request.Headers.ContainsKey("True-Client-IP") && !string.IsNullOrEmpty(Request.Headers["True-Client-IP"])) sIpAddress = Request.Headers["True-Client-IP"]; //if the user is behind a proxy server
                if (sIpAddress == "")
                    if (Request.Headers.ContainsKey("CF-CONNECTING-IP") && !string.IsNullOrEmpty(Request.Headers["CF-CONNECTING-IP"]))
                        sIpAddress = Request.Headers["CF-CONNECTING-IP"];
                if (sIpAddress == "")
                    if (Request.Headers.ContainsKey("X-Forwarded-For") && !string.IsNullOrEmpty(Request.Headers["X-Forwarded-For"]))
                        sIpAddress = Request.Headers["X-Forwarded-For"];
                if (sIpAddress == "") sIpAddress = HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
            }
            catch (Exception ex)
            {
                Core.Log.Logger.Info($"{MethodBase.GetCurrentMethod()?.Name}: {ex.Message} - {ex.InnerException} - {ex.StackTrace}");
            }
            return sIpAddress;
        }

        /// <summary>
        /// Đăng nhập bằng Google OAuth
        /// </summary>
        [HttpPost("google-login")]
        public IActionResult GoogleLogin([FromBody] GoogleLoginRequest request)
        {
            var retval = new ReturnBaseInfo<AuthResponseInfo>();
            retval.ReturnStatus = new StatusBaseInfo { Message = "Không thành công", Code = 0 };
            retval.ReturnData = null;
            
            if (string.IsNullOrEmpty(request.IdToken))
            {
                retval.ReturnStatus.Code = -1;
                retval.ReturnStatus.Message = "Token không được cung cấp";
                return Ok(retval);
            }

            try
            {
                // Phân tích JWT token mà không cần xác thực Google
                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadJwtToken(request.IdToken);
                
                // Lấy thông tin từ token
                var userEmail = jwtToken.Claims.FirstOrDefault(c => c.Type == "email")?.Value;
                var fullName = jwtToken.Claims.FirstOrDefault(c => c.Type == "name")?.Value;
                var googleUserId = jwtToken.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;
                
                if (string.IsNullOrEmpty(userEmail) || string.IsNullOrEmpty(googleUserId))
                {
                    retval.ReturnStatus.Code = -1;
                    retval.ReturnStatus.Message = "Không thể lấy thông tin người dùng từ token";
                    return Ok(retval);
                }
                
                // Kiểm tra xem người dùng đã tồn tại trong hệ thống chưa (kiểm tra bằng email)
                var userInfo = ServiceFactory.User.GetUserByEmail(userEmail);
                
                if (userInfo.Code != 1 || userInfo.Result == null)
                {
                    // Nếu không tìm thấy email, tạo người dùng mới
                    var userName = GenerateUsernameFromEmail(userEmail);
                    var randomPassword = GenerateRandomPassword();
                    var hashedPassword = Util.HashPassword(randomPassword);
                    
                    var newUser = new UserInfo
                    {
                        Username = userName,
                        Email = userEmail,
                        Full_Name = fullName ?? userEmail,
                        Password_Hash = hashedPassword,
                        Is_Admin = false,
                        Is_Active = true
                    };
                    
                    var createResult = ServiceFactory.User.CreateUser(newUser);
                    if (createResult.Code != 1 || createResult.Result <= 0)
                    {
                        retval.ReturnStatus.Message = "Không thể tạo tài khoản mới";
                        return Ok(retval);
                    }
                    
                    // Lấy lại thông tin người dùng vừa tạo
                    userInfo = ServiceFactory.User.GetUserById(createResult.Result);
                    if (userInfo.Code != 1 || userInfo.Result == null)
                    {
                        retval.ReturnStatus.Message = "Không thể lấy thông tin người dùng vừa tạo";
                        return Ok(retval);
                    }
                }
                else if (!userInfo.Result.Is_Active)
                {
                    // Nếu tài khoản bị khóa
                    retval.ReturnStatus.Message = "Tài khoản đã bị khóa hoặc không hoạt động";
                    return Ok(retval);
                }

                // Tạo JWT token cho người dùng đăng nhập
                var token = "";
                var refreshToken = "";
                var forceLogoutKey = Guid.NewGuid().ToString();
                setNewToken(userInfo.Result, forceLogoutKey, out token, out refreshToken);

                // Trả về thông tin người dùng và token
                var objectData = new AuthResponseInfo
                {
                    AccessToken = token,
                    Id = userInfo.Result.Id,
                    RefreshToken = refreshToken,
                    Is_Admin = userInfo.Result.Is_Admin,
                    Username = userInfo.Result.Username,
                    Full_Name = userInfo.Result.Full_Name
                };

                retval.ReturnData = objectData;
                retval.ReturnStatus.Code = 1;
                retval.ReturnStatus.Message = "Đăng nhập bằng Google thành công";
                
                return Ok(retval);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi đăng nhập bằng Google");
                retval.ReturnStatus.Message = $"Lỗi hệ thống: {ex.Message}";
                return Ok(retval);
            }
        }
        
        /// <summary>
        /// Phân tích URL hash để lấy các tham số
        /// </summary>
        private Dictionary<string, string> ParseUrlHash(string urlHash)
        {
            var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            
            // Loại bỏ dấu # ở đầu nếu có
            if (urlHash.StartsWith("#"))
            {
                urlHash = urlHash.Substring(1);
            }
            
            // Phân tách các tham số
            string[] pairs = urlHash.Split('&');
            foreach (string pair in pairs)
            {
                int idx = pair.IndexOf('=');
                if (idx > 0)
                {
                    string key = Uri.UnescapeDataString(pair.Substring(0, idx));
                    string value = Uri.UnescapeDataString(pair.Substring(idx + 1));
                    result[key] = value;
                }
            }
            
            return result;
        }
        
        /// <summary>
        /// Tạo username từ địa chỉ email
        /// </summary>
        private string GenerateUsernameFromEmail(string email)
        {
            // Lấy phần trước @ làm username cơ bản
            var baseUsername = email.Split('@')[0];
            
            // Loại bỏ các ký tự không hợp lệ
            baseUsername = System.Text.RegularExpressions.Regex.Replace(baseUsername, @"[^a-zA-Z0-9_]", "");
            
            // Thêm prefix "google_" để dễ nhận biết
            return "google_" + baseUsername;
        }
        
        /// <summary>
        /// Tạo mật khẩu ngẫu nhiên cho người dùng đăng ký từ Google
        /// </summary>
        private string GenerateRandomPassword(int length = 12)
        {
            const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789!@#$%^&*()";
            var random = new Random();
            
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
