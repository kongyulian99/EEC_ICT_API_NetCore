using ApiService.Common;
using ApiService.Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;

namespace ApiService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [ServiceFilter(typeof(MyActionFilter))]
    public abstract class MyBaseController<T> : ControllerBase where T : MyBaseController<T>
    {
        public readonly ILogger<T> _logger;
        public AppSetting _appSettingInfo;        
        protected MyBaseController(ILogger<T> logger, AppSetting appSettingInfo) {
            _logger = logger;
            _appSettingInfo = appSettingInfo;
        }
        protected string GetUserNameCurrent()
        {
            var reqToken = HttpContext.Request.Headers.Where(it => it.Key == "Authorization").FirstOrDefault();
            var handler = new JwtSecurityTokenHandler();
            try {
                var jwtSecurityToken = handler.ReadJwtToken(reqToken.Value.FirstOrDefault().Replace("bearer ", ""));
                var userName = jwtSecurityToken.Claims.First(claim => claim.Type == "UserName").Value;
                return userName;
            } catch { 
                return "";
            }
        }
    }
}
