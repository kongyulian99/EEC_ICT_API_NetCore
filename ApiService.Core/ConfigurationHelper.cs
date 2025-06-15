using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace ApiService.Core
{
    public static class ConfigurationHelper
    {
        public static IConfiguration config;
        public static string connectString;
        public static string connectStringDb1;
        public static string connectStringDb2;
        public static void Initialize(IConfiguration Configuration)
        {
            config = Configuration;
            connectString = ConnectString();
            connectStringDb1 = ConnectString("DB1");
            connectStringDb2 = ConnectString("DB2");
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dbKey">Lay key Db trong appsetting.json</param>
        /// <returns></returns>
        private static string ConnectString(string dbKey = "DefaultConnection")
        {
            return config.GetSection("AppSetting:DataBaseConnectString:" + dbKey).Value;
        }
    }
}
