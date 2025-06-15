using ApiService.Business;
using ApiService.Entity;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiService.Common
{
    public class HttpRequestMidleware
    {
        private readonly RequestDelegate next;
        private readonly ILogger<HttpRequestMidleware> _logger;
        private readonly AppSetting _appsetting;
        public HttpRequestMidleware(RequestDelegate next
            , ILogger<HttpRequestMidleware> logger
            , AppSetting appsetting)
        {
            this.next = next;
            this._logger = logger;
            _appsetting = appsetting;
        }
        public async Task Invoke(HttpContext context)
        {
            var remoteIpAddress = context.Connection.RemoteIpAddress.ToString();
            var reqId = Guid.NewGuid().ToString();
            var userName = "";
            try
            {
                var reqToken = context.Request.Headers.Where(it => it.Key == "Authorization").FirstOrDefault();
                if (!reqToken.Equals(new KeyValuePair<string, StringValues>()))
                {
                    var handler = new JwtSecurityTokenHandler();
                    var jwtSecurityToken = handler.ReadJwtToken(reqToken.Value.FirstOrDefault().Replace("bearer ", ""));
                    userName = jwtSecurityToken.Claims.First(claim => claim.Type == "UserName").Value;
                }
            }
            catch { }
            var mess = $"[[{remoteIpAddress}-{userName}]-RequestId-[{reqId}]]-" + context.Request.Path;

            //var userInfo = ServiceFactory.User.GetUserInfo(userName);

            context.Request.EnableBuffering();
            var buffer = new byte[Convert.ToInt32(context.Request.ContentLength)];
            await context.Request.Body.ReadAsync(buffer, 0, buffer.Length);
            var requestBody = Encoding.UTF8.GetString(buffer);
            context.Request.Body.Seek(0, SeekOrigin.Begin);

            if (_appsetting.MiddlewareConfig.Tracing)
            {
                try
                {
                    dynamic jsonObj = Newtonsoft.Json.JsonConvert.DeserializeObject(requestBody);
                    try { if (jsonObj["password"] != null) jsonObj["password"] = "*****"; } catch { }
                    mess = mess + $"{Environment.NewLine}" + Newtonsoft.Json.JsonConvert.SerializeObject(jsonObj, Newtonsoft.Json.Formatting.Indented);
                }
                catch
                {
                    mess = mess + $"{Environment.NewLine}" + requestBody;
                }
                _logger.LogInformation(mess);
            }
            var originalBodyStream = context.Response.Body;
            using (var responseBody = new MemoryStream())
            {
                context.Response.Body = responseBody;
                await next(context);

                context.Response.Body.Seek(0, SeekOrigin.Begin);
                var response = await new StreamReader(context.Response.Body).ReadToEndAsync();
                context.Response.Body.Seek(0, SeekOrigin.Begin);
                if (_appsetting.MiddlewareConfig.Tracing)
                {
                    try
                    {
                        //không lưu log nội dung file downdoad!l
                        if (context.Response.ContentType != null && !context.Response.ContentType.Equals("application/octet-stream"))
                        {
                            _logger.LogInformation($"[Response-RequestId-[{reqId}]]{Environment.NewLine}" + response);
                        }
                        else
                        {
                            _logger.LogInformation($"[Response-RequestId-[{reqId}]][File]");
                        }
                    }
                    catch { _logger.LogInformation($"[Response-RequestId-[{reqId}]]{Environment.NewLine}" + response); }
                }
                await responseBody.CopyToAsync(originalBodyStream);
            }
        }
    }
}
