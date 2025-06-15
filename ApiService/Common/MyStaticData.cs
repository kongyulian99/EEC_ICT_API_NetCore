using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using ApiService.Core.RedisHelper.Services;
using ApiService.Business;

namespace ApiService.Common
{
    public static class MyStaticData
    {
        public static void AddDataToCache(this IHost host)
        {
            var services = host.Services.CreateScope();
            var _memoryCache = services.ServiceProvider.GetService<IMemoryCache>();
            //var _redisCache = service.ServiceProvider.GetService<IRedisClientService>();
            //var lstMappingCommand = ServiceFactory.MappingCommand.GetListMappingCommand().Result;
            //var mappingCommandKey = MyConstant.MappingCommandCacheKey;
            //_memoryCache.Remove(mappingCommandKey);
            //_memoryCache.Set(mappingCommandKey, lstMappingCommand);

        }
    }
}
