using ApiService.Business;
using ApiService.Common;
using ApiService.Core;
using ApiService.Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiService.Controllers
{
    [Route("api/{controller}/{action}")]
    [ApiController]    
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };
        private AuthService _authSv;
        IMemoryCache _memoryCache;
        ILogger<WeatherForecastController> _logger;
        AppSetting _appSettingInfo;
        public WeatherForecastController(
            //DI default service
            ILogger<WeatherForecastController> logger
            , AppSetting appSettingInfo
            //DI services
            , AuthService authSv
            , IMemoryCache memoryCache
         )  
        {
            _logger = logger;
            _appSettingInfo = appSettingInfo;
            _authSv = authSv;
            _memoryCache = memoryCache;
        }

        [HttpGet]
        public IEnumerable<WeatherForecast> Get()
        {
            //var uinfo = new UserInfo();
            //uinfo.cUserName = "hungnd";
            //uinfo.cUserFullName = "Nguyen Van A";
            //uinfo.cSalt = Guid.NewGuid().ToString();
            //uinfo.cStatus = 1;
            //uinfo.cForceLogoutKey = Guid.NewGuid().ToString();
            //var password = "123456" + uinfo.cSalt;
            //uinfo.cPasssword = Util.CreateMD5(password);
            //var c =ServiceFactory.User.UpdateUser(uinfo);
            //var t = 1;

            //var cc1 = ConfigurationHelper.connectString;
            //_logger.LogInformation("Test log");
            //var cacheUser = MyConstant.UserRoleCacheKey + "xxxx";
            //var vvv = _memoryCache.Get(cacheUser);

            //_memoryCache.Set(cacheUser, "xxxx");
            //var vvv = _memoryCache.Get(MyConstant.MappingCommandCacheKey);

            //var ccc1 = _authSv.CheckLogin("xl");
            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();
        }
    }
}
