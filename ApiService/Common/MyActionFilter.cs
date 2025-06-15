using ApiService.Business;
using ApiService.Entity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ApiService.Common
{
    public class MyActionFilter : ActionFilterAttribute
    {
        private readonly ILogger<MyActionFilter> _logger;
        private readonly AppSetting _appSettingInfo;
        private readonly IMemoryCache _memoryCache;
        public MyActionFilter(ILogger<MyActionFilter> logger
            , AppSetting appSettingInfo
            , IMemoryCache memoryCache)
        {
            _logger = logger;
            _appSettingInfo = appSettingInfo;
            _memoryCache = memoryCache;
        }

        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            try
            {
                if (!(context.ActionDescriptor is ControllerActionDescriptor controllerActionDescriptor))
                {
                    context.Result = new UnauthorizedResult();
                    return;
                }
                var controllerName = controllerActionDescriptor.ControllerName;
                var actionName = controllerActionDescriptor.ActionName;

                 
                var reqToken = context.HttpContext.Request.Headers.Where(it=>it.Key == "Authorization").FirstOrDefault();
                if (reqToken.Equals(new KeyValuePair<string, StringValues>()))
                {
                    context.Result = new UnauthorizedResult();
                    return;
                }
                var handler = new JwtSecurityTokenHandler();
                var jwtSecurityToken = handler.ReadJwtToken(reqToken.Value.FirstOrDefault().Replace("bearer ","").Replace("Bearer ", ""));
                var username = jwtSecurityToken.Claims.First(claim => claim.Type == "UserName").Value;
                var userId = int.Parse(jwtSecurityToken.Claims.First(claim => claim.Type == "UserId").Value);
                var isAdmin = bool.Parse(jwtSecurityToken.Claims.First(claim => claim.Type == "IsAdmin").Value);

                // Kiểm tra thông tin người dùng từ cache hoặc database
                var userInfo = _memoryCache.Get<UserInfo>(MyConstant.UserInfoCacheKey + username);
                if (userInfo == null)
                {
                    var userInfoDb = ServiceFactory.User.GetUserByUsername(username);
                    if (userInfoDb == null || userInfoDb.Code != 1 || userInfoDb.Result == null)
                    {
                        context.Result = new UnauthorizedResult();
                        return;
                    }
                    userInfo = userInfoDb.Result;
                    
                    // Lưu vào cache
                    _memoryCache.Set(MyConstant.UserInfoCacheKey + username, userInfo, 
                        new MemoryCacheEntryOptions().SetSlidingExpiration(
                            TimeSpan.FromMinutes(_appSettingInfo.Jwt.TokenLifeTime)));
                }

                // Kiểm tra trạng thái người dùng
                if (!userInfo.Is_Active)
                {
                    context.Result = new BadRequestObjectResult(new ReturnBaseInfo<object>()
                    {
                        ReturnStatus = new StatusBaseInfo { Message = "Tài khoản đã bị khóa hoặc không hoạt động!", Code = -1 },
                        ReturnData = null
                    });
                    return;
                }

                // Kiểm tra quyền admin nếu cần
                if (RequireAdminAccess(controllerName, actionName))
                {
                    if (!userInfo.Is_Admin)
                    {
                        context.Result = new BadRequestObjectResult(new ReturnBaseInfo<object>()
                        {
                            ReturnStatus = new StatusBaseInfo { Message = "Bạn không có quyền thực hiện hành động này!", Code = -1 },
                            ReturnData = null
                        });
                        return;
                    }
                }

                // Kiểm tra quyền theo Command nếu có
                if (controllerName.Equals("User", StringComparison.OrdinalIgnoreCase) && 
                    actionName.Equals("CommandProcess", StringComparison.OrdinalIgnoreCase))
                {
                    string body = "";
                    context.HttpContext.Request.EnableBuffering();
                    context.HttpContext.Request.Body.Position = 0;

                    using (var reader = new StreamReader(context.HttpContext.Request.Body))
                    {
                        body = await reader.ReadToEndAsync();
                        context.HttpContext.Request.Body.Position = 0;
                    }

                    if (!string.IsNullOrEmpty(body))
                    {
                        var reqModel = JsonConvert.DeserializeObject<RequestInfo>(body);
                        if (reqModel != null && !string.IsNullOrEmpty(reqModel.Command))
                        {
                            // Kiểm tra quyền admin cho các command cần quyền admin
                            if (RequireAdminForCommand(reqModel.Command) && !userInfo.Is_Admin)
                            {
                                context.Result = new BadRequestObjectResult(new ReturnBaseInfo<object>()
                                {
                                    ReturnStatus = new StatusBaseInfo { Message = "Bạn không có quyền thực hiện hành động này!", Code = -1 },
                                    ReturnData = null
                                });
                                return;
                            }
                        }
                    }
                }

                await next();                
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + System.Environment.NewLine + ex.StackTrace);
                context.Result = new BadRequestObjectResult(new ReturnBaseInfo<object>()
                {
                    ReturnStatus = new StatusBaseInfo { Message = "Exception", Code = -1 },
                    ReturnData = null
                });
                return;
            }
        }

        /// <summary>
        /// Kiểm tra xem controller và action có yêu cầu quyền admin không
        /// </summary>
        private bool RequireAdminAccess(string controllerName, string actionName)
        {
            // Danh sách các controller và action yêu cầu quyền admin
            if (controllerName.Equals("User", StringComparison.OrdinalIgnoreCase))
            {
                return true; // Tất cả các action trong UserController đều yêu cầu quyền admin
            }

            return false;
        }

        /// <summary>
        /// Kiểm tra xem command có yêu cầu quyền admin không
        /// </summary>
        private bool RequireAdminForCommand(string command)
        {
            // Danh sách các command yêu cầu quyền admin
            var adminCommands = new[]
            {
                "USER_CREATE",
                "USER_UPDATE",
                "USER_DELETE",
                "USER_UPDATE_PASSWORD"
            };

            return adminCommands.Contains(command, StringComparer.OrdinalIgnoreCase);
        }
    }
}
