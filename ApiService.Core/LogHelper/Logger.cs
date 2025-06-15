using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Reflection.Metadata;
using System.Reflection;
using System.Text;
using log4net.Config;
using log4net;
using System.Threading;

namespace ApiService.Core.Log
{
    public static class Logger
    {
        private static readonly ILogService logService = new Log4NetService();

        public static void Error(Exception ex)
        {
            logService.Error(ex);
        }
        public static void Info(string message)
        {
            logService.Information(message);
        }
        public static void InfoFormat(string message, string[] dataformat)
        {
            var infolog = string.Format(message, dataformat);
            logService.Information(infolog);
        }
        public static void Debug(string message)
        {
            logService.Debug(message);
        }
    }
}
