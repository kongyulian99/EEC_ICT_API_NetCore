using ApiService.Core.RedisHelper.Factories;
using ApiService.Core.RedisHelper.Services;
using Autofac;
using Microsoft.Extensions.Configuration;

namespace ApiService.Core.RedisHelper.Configurations
{
    public class RedisClientModule : Module
    {        
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            builder.RegisterType<RedisConnectionFactory>().As(typeof(IRedisConnectionFactory)).SingleInstance();
            builder.RegisterType<RedisClientService>().As(typeof(IRedisClientService)).SingleInstance();
        }
    }
}
