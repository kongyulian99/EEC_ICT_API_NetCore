using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text;

namespace ApiService.Core.RedisHelper.Factories
{
    public interface IRedisConnectionFactory
    {
        ConnectionMultiplexer GetConnection();
    }
}
