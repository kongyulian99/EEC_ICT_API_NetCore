using System;
using System.Collections.Generic;
using System.Text;

namespace ApiService.Core.RedisHelper.Configurations
{
    public class RedisClientSetting
    {
        /// <summary>
        /// Redis database , default value is 0
        /// </summary>
        public int DbNumber { get; set; } = 0;

        /// <summary>
        /// Connection string which without Db number
        /// </summary>
        public string RedisConnectionString { get; set; }
    }
}
