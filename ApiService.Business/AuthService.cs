using ApiService.Core;
using ApiService.Core.Log;
using ApiService.Entity;
using ApiService.Implement;
using ApiService.Interface;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace ApiService.Business
{
    public class AuthService : ServiceBase<AuthService>
    {
        public AuthService(AppSetting appSettingInfo) : base(appSettingInfo)
        {
        }
        public ReturnBaseInfo<UserInfo> CheckLogin(string username, string password)
        {
            var retval = new ReturnBaseInfo<UserInfo>();
            retval.ReturnStatus = new StatusBaseInfo { Message = "Không thành công", Code = 0 };
            retval.ReturnData = null;

            var userInfo = ServiceFactory.User.GetUserByUsername(username);
            //Sai thông tin đăng nhập
            if(userInfo.Code != 1 || userInfo.Result == null)
            {
                retval.ReturnStatus.Message = "Sai thông tin đăng nhập!";
                return retval;
            }
            //Tài khoản không hoạt động
            if (!userInfo.Result.Is_Active)
            {
                retval.ReturnStatus.Message = "Tài khoản đã bị khóa hoặc không hoạt động!";
                return retval;
            }
            //Kiểm tra mật khẩu
            if (!Util.VerifyPassword(password, userInfo.Result.Password_Hash))
            {                
                retval.ReturnStatus.Message = "Sai thông tin đăng nhập!";                
                return retval;
            }
            retval.ReturnStatus.Message = "Xác thực thành công!";
            retval.ReturnStatus.Code = 1;
            retval.ReturnData = userInfo.Result;
            return retval;
        }

        

        public ReturnBaseInfo<int> UpdateLastLogin(string username)
        {
            var retval = new ReturnBaseInfo<int>();
            retval.ReturnStatus = new StatusBaseInfo { Message = "Không thành công", Code = 0 };
            retval.ReturnData = 0;

            var userInfo = ServiceFactory.User.GetUserByUsername(username);
            //Sai thông tin người dùng
            if (userInfo.Code != 1 || userInfo.Result == null)
            {
                retval.ReturnStatus.Message = "Không tìm thấy thông tin người dùng!";
                return retval;
            }
            //Tài khoản không hoạt động
            if (!userInfo.Result.Is_Active)
            {
                retval.ReturnStatus.Message = "Tài khoản đã bị khóa hoặc không hoạt động!";
                return retval;
            }
            
            //Cập nhật thời gian đăng nhập
            var updateResult = ServiceFactory.User.UpdateLastLogin(userInfo.Result.Id);
            
            if (updateResult.Code == 1)
            {
                retval.ReturnStatus.Message = "Thành công!";
                retval.ReturnStatus.Code = 1;
                retval.ReturnData = updateResult.Result;
            }
            else
            {
                retval.ReturnStatus.Message = updateResult.Message;
            }
            
            return retval;
        }

        public bool UpdateUser(UserInfo info)
        {
            var result = ServiceFactory.User.UpdateUser(info);
            return result.Code == 1;
        }
    }
}
