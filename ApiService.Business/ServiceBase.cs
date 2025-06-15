using ApiService.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace ApiService.Business
{
    public abstract class ServiceBase<T> where T : ServiceBase<T>
    {
        public AppSetting _appSettingInfo;
        public ServiceBase(AppSetting appSettingInfo)
        {
            _appSettingInfo = appSettingInfo;
        }
    }
}
