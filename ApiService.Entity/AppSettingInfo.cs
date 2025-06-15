using System;
using System.Collections.Generic;
using System.Text;

namespace ApiService.Entity
{
    public class AppSetting
    {
        public DataBaseConnectString DataBaseConnectString { get; set; }
        public Jwt Jwt { get; set; }       
        public MiddlewareConfig MiddlewareConfig { get; set; }
    }
    public class MiddlewareConfig
    {
        public bool Tracing { get; set; }
    }
    public class Jwt
    {
        public string Key { get; set; }
        public string Issuer { get; set; }
        public int TokenLifeTime { get; set; }
        public int RefreshTokenAppendTime { get; set; }
    }
    public class DataBaseConnectString
    {
        public string DefaultConnection { get; set; }
        public string DB1 { get; set; }
        public string DB2 { get; set; }
    }
}
