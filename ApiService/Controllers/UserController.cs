using ApiService.Business;
using ApiService.Common;
using ApiService.Core;
using ApiService.Entity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;

namespace ApiService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : MyBaseController<UserController>
    {
        private AuthService _authSv;
        private readonly IMemoryCache _memoryCache;

        public UserController(
            //DI default service
            ILogger<UserController> logger
            , AppSetting appSettingInfo
            //DI services
            , AuthService authSv
            , IMemoryCache memoryCache
         ) : base(logger, appSettingInfo)
        {
            _authSv = authSv;
            _memoryCache = memoryCache;
        }
        
        /// <summary>
        /// Đăng xuất người dùng hiện tại
        /// </summary>
        [HttpPost("Logout")]
        public IActionResult Logout()
        {
            var retval = new ReturnBaseInfo<object>();
            retval.ReturnStatus = new StatusBaseInfo { Message = "Không thành công", Code = 0 };
            retval.ReturnData = null;
            
            var username = GetUserNameCurrent();
            
            // Cập nhật thời gian đăng nhập cuối cùng
            var userInfo = ServiceFactory.User.GetUserByUsername(username);
            if (userInfo.Code == 1 && userInfo.Result != null)
            {
                ServiceFactory.User.UpdateLastLogin(userInfo.Result.Id);
            }
            
            // Xóa cache
            var cacheUserInfoKey = MyConstant.UserInfoCacheKey + username;
            _memoryCache.Remove(cacheUserInfoKey);
            
            retval.ReturnStatus.Code = 1;
            retval.ReturnStatus.Message = "Đăng xuất thành công";
            return Ok(retval);
        }
        
        /// <summary>
        /// Lấy danh sách tất cả người dùng
        /// </summary>
        [HttpGet("GetAll")]
        public IActionResult GetAllUsers()
        {
            var retval = new ReturnBaseInfo<List<UserInfo>>();
            retval.ReturnStatus = new StatusBaseInfo { Message = "Không thành công", Code = 0 };
            
            var lstUser = ServiceFactory.User.GetAllUsers();
            retval.ReturnData = lstUser.Result;
            retval.ReturnStatus.Code = lstUser.Code;
            retval.ReturnStatus.Message = lstUser.Code == 1 ? "Lấy danh sách người dùng thành công" : lstUser.Message;
            
            return Ok(retval);
        }
        
        /// <summary>
        /// Lấy thông tin người dùng theo ID
        /// </summary>
        [HttpGet("GetById/{id}")]
        public IActionResult GetUserById(int id)
        {
            var retval = new ReturnBaseInfo<UserInfo>();
            retval.ReturnStatus = new StatusBaseInfo { Message = "Không thành công", Code = 0 };
            
            if (id <= 0)
            {
                retval.ReturnStatus.Message = "ID người dùng không hợp lệ";
                return Ok(retval);
            }
            
            var user = ServiceFactory.User.GetUserById(id);
            retval.ReturnData = user.Result;
            retval.ReturnStatus.Code = user.Code;
            retval.ReturnStatus.Message = user.Code == 1 ? "Lấy thông tin người dùng thành công" : user.Message;
            
            return Ok(retval);
        }
        
        /// <summary>
        /// Lấy thông tin người dùng theo tên đăng nhập
        /// </summary>
        [HttpGet("GetByUsername/{username}")]
        public IActionResult GetUserByUsername(string username)
        {
            var retval = new ReturnBaseInfo<UserInfo>();
            retval.ReturnStatus = new StatusBaseInfo { Message = "Không thành công", Code = 0 };
            
            if (string.IsNullOrEmpty(username))
            {
                retval.ReturnStatus.Message = "Tên đăng nhập không hợp lệ";
                return Ok(retval);
            }
            
            var user = ServiceFactory.User.GetUserByUsername(username);
            retval.ReturnData = user.Result;
            retval.ReturnStatus.Code = user.Code;
            retval.ReturnStatus.Message = user.Code == 1 ? "Lấy thông tin người dùng thành công" : user.Message;
            
            return Ok(retval);
        }
        
        /// <summary>
        /// Tạo người dùng mới
        /// </summary>
        [HttpPost("Create")]
        public IActionResult CreateUser([FromBody] UserInfo user)
        {
            var retval = new ReturnBaseInfo<int>();
            retval.ReturnStatus = new StatusBaseInfo { Message = "Không thành công", Code = 0 };
            
            if (user == null)
            {
                retval.ReturnStatus.Message = "Dữ liệu không hợp lệ";
                return Ok(retval);
            }

            user.Password_Hash = Util.HashPassword(user.Password);
            
            var createResult = ServiceFactory.User.CreateUser(user);
            retval.ReturnData = createResult.Result;
            retval.ReturnStatus.Code = createResult.Code;
            retval.ReturnStatus.Message = createResult.Message;
            
            return Ok(retval);
        }
        
        /// <summary>
        /// Cập nhật thông tin người dùng
        /// </summary>
        [HttpPut("Update")]
        public IActionResult UpdateUser([FromBody] UserInfo user)
        {
            var retval = new ReturnBaseInfo<int>();
            retval.ReturnStatus = new StatusBaseInfo { Message = "Không thành công", Code = 0 };
            
            if (user == null || user.Id <= 0)
            {
                retval.ReturnStatus.Message = "Dữ liệu không hợp lệ";
                return Ok(retval);
            }

            user.Password_Hash = Util.HashPassword(user.Password);

            var updateResult = ServiceFactory.User.UpdateUser(user);
            retval.ReturnData = updateResult.Result;
            retval.ReturnStatus.Code = updateResult.Code;
            retval.ReturnStatus.Message = updateResult.Message;
            
            return Ok(retval);
        }
        
        /// <summary>
        /// Xóa người dùng
        /// </summary>
        [HttpDelete("Delete/{id}")]
        public IActionResult DeleteUser(int id)
        {
            var retval = new ReturnBaseInfo<int>();
            retval.ReturnStatus = new StatusBaseInfo { Message = "Không thành công", Code = 0 };
            
            if (id <= 0)
            {
                retval.ReturnStatus.Message = "ID người dùng không hợp lệ";
                return Ok(retval);
            }
            
            var deleteResult = ServiceFactory.User.DeleteUser(id);
            retval.ReturnData = deleteResult.Result;
            retval.ReturnStatus.Code = deleteResult.Code;
            retval.ReturnStatus.Message = deleteResult.Message;
            
            return Ok(retval);
        }
        
        /// <summary>
        /// Cập nhật mật khẩu người dùng
        /// </summary>
        [HttpPut("UpdatePassword")]
        public IActionResult UpdateUserPassword([FromBody] PasswordUpdateModel model)
        {
            var retval = new ReturnBaseInfo<int>();
            retval.ReturnStatus = new StatusBaseInfo { Message = "Không thành công", Code = 0 };
            
            if (model == null || model.UserId <= 0 || string.IsNullOrEmpty(model.NewPasswordHash))
            {
                retval.ReturnStatus.Message = "Dữ liệu không hợp lệ";
                return Ok(retval);
            }
            
            var passwordResult = ServiceFactory.User.UpdateUserPassword(model.UserId, model.NewPasswordHash);
            retval.ReturnData = passwordResult.Result;
            retval.ReturnStatus.Code = passwordResult.Code;
            retval.ReturnStatus.Message = passwordResult.Message;
            
            return Ok(retval);
        }
    }
    
    public class PasswordUpdateModel
    {
        public int UserId { get; set; }
        public string NewPasswordHash { get; set; }
    }
}
