using ApiService.Core.RedisHelper.Configurations;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text;

namespace ApiService.Core.RedisHelper.Factories
{
    public class RedisConnectionFactory : IRedisConnectionFactory
    {
        private static Lazy<ConnectionMultiplexer> Connection;

        public RedisConnectionFactory(RedisClientSetting redisSetting)
        {
            var options = ConfigurationOptions.Parse(redisSetting.RedisConnectionString);
            Connection = new Lazy<ConnectionMultiplexer>(() => ConnectionMultiplexer.Connect(options));
        }

        public ConnectionMultiplexer GetConnection() => Connection.Value;
    }
}
