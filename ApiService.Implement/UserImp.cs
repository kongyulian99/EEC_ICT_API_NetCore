using ApiService.Core;
using ApiService.Core.DataHelper;
using ApiService.Core.Log;
using ApiService.Entity;
using ApiService.Interface;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace ApiService.Implement
{
    public class UserImp : IUser
    {
        public DbReturnInfo<int> CreateUser(UserInfo user)
        {
            var retval = new DbReturnInfo<int>();
            MySqlParameter[] param = {
                new MySqlParameter("@sUsername", user.Username),
                new MySqlParameter("@sPassword_Hash", user.Password_Hash),
                new MySqlParameter("@sEmail", user.Email),
                new MySqlParameter("@sFull_Name", user.Full_Name ?? (object)DBNull.Value),
                new MySqlParameter("@bIs_Admin", user.Is_Admin)
            };
            
            string outVal = "";
            var dt = MySQLDataHelper.GetDataTable(ConfigurationHelper.connectString, "CreateUser", param, out outVal);
            
            if (dt != null && dt.Rows.Count > 0)
            {
                retval.Result = Convert.ToInt32(dt.Rows[0]["New_User_Id"]);
                retval.Code = 1;
                retval.Message = "Tạo người dùng thành công";
            }
            else
            {
                retval.Code = 0;
                retval.Message = "Tạo người dùng thất bại";
            }
            
            return retval;
        }

        public DbReturnInfo<UserInfo> GetUserById(int id)
        {
            var retval = new DbReturnInfo<UserInfo>();
            MySqlParameter[] param = {
                new MySqlParameter("@iId", id)
            };
            
            string outVal = "";
            var users = MySQLDataHelper.ExecuteReaderToList<UserInfo>(ConfigurationHelper.connectString, "GetUserById", param, out outVal);
            
            if (users != null && users.Count > 0)
            {
                retval.Result = users.FirstOrDefault();
                retval.Code = 1;
                retval.Message = "Thành công";
            }
            else
            {
                retval.Code = 0;
                retval.Message = "Không tìm thấy người dùng";
            }
            
            return retval;
        }

        public DbReturnInfo<UserInfo> GetUserByUsername(string username)
        {
            var retval = new DbReturnInfo<UserInfo>();
            MySqlParameter[] param = {
                new MySqlParameter("@sUsername", username)
            };
            
            string outVal = "";
            var users = MySQLDataHelper.ExecuteReaderToList<UserInfo>(ConfigurationHelper.connectString, "GetUserByUsername", param, out outVal);
            
            if (users != null && users.Count > 0)
            {
                retval.Result = users.FirstOrDefault();
                retval.Code = 1;
                retval.Message = "Thành công";
            }
            else
            {
                retval.Code = 0;
                retval.Message = "Không tìm thấy người dùng";
            }
            
            return retval;
        }

        public DbReturnInfo<int> UpdateUser(UserInfo user)
        {
            var retval = new DbReturnInfo<int>();
            MySqlParameter[] param = {
                new MySqlParameter("@iId", user.Id),
                new MySqlParameter("@sEmail", user.Email),
                new MySqlParameter("@sFull_Name", user.Full_Name ?? (object)DBNull.Value),
                new MySqlParameter("@bIs_Admin", user.Is_Admin),
                new MySqlParameter("@bIs_Active", user.Is_Active)
            };
            
            string outVal = "";
            var result = MySQLDataHelper.ExecuteNonQuery(ConfigurationHelper.connectString, "UpdateUser", param, out outVal);
            
            if (result >= 0)
            {
                retval.Result = result;
                retval.Code = 1;
                retval.Message = "Cập nhật người dùng thành công";
            }
            else
            {
                retval.Code = 0;
                retval.Message = "Cập nhật người dùng thất bại";
            }
            
            return retval;
        }

        public DbReturnInfo<int> UpdateUserPassword(int id, string newPasswordHash)
        {
            var retval = new DbReturnInfo<int>();
            MySqlParameter[] param = {
                new MySqlParameter("@iId", id),
                new MySqlParameter("@sNew_Password_Hash", newPasswordHash)
            };
            
            string outVal = "";
            var result = MySQLDataHelper.ExecuteNonQuery(ConfigurationHelper.connectString, "UpdateUserPassword", param, out outVal);
            
            if (result >= 0)
            {
                retval.Result = result;
                retval.Code = 1;
                retval.Message = "Cập nhật mật khẩu thành công";
            }
            else
            {
                retval.Code = 0;
                retval.Message = "Cập nhật mật khẩu thất bại";
            }
            
            return retval;
        }

        public DbReturnInfo<int> DeleteUser(int id)
        {
            var retval = new DbReturnInfo<int>();
            MySqlParameter[] param = {
                new MySqlParameter("@iId", id)
            };
            
            string outVal = "";
            var result = MySQLDataHelper.ExecuteNonQuery(ConfigurationHelper.connectString, "DeleteUser", param, out outVal);
            
            if (result >= 0)
            {
                retval.Result = result;
                retval.Code = 1;
                retval.Message = "Xóa người dùng thành công";
            }
            else
            {
                retval.Code = 0;
                retval.Message = "Xóa người dùng thất bại";
            }
            
            return retval;
        }

        public DbReturnInfo<int> UpdateLastLogin(int id)
        {
            var retval = new DbReturnInfo<int>();
            MySqlParameter[] param = {
                new MySqlParameter("@iId", id)
            };
            
            string outVal = "";
            var result = MySQLDataHelper.ExecuteNonQuery(ConfigurationHelper.connectString, "UpdateLastLogin", param, out outVal);
            
            if (result >= 0)
            {
                retval.Result = result;
                retval.Code = 1;
                retval.Message = "Cập nhật thời gian đăng nhập thành công";
            }
            else
            {
                retval.Code = 0;
                retval.Message = "Cập nhật thời gian đăng nhập thất bại";
            }
            
            return retval;
        }

        public DbReturnInfo<List<UserInfo>> GetAllUsers()
        {
            var retval = new DbReturnInfo<List<UserInfo>>();
            string outVal = "";
            var users = MySQLDataHelper.ExecuteReaderToList<UserInfo>(ConfigurationHelper.connectString, "GetAllUsers", null, out outVal);
            
            if (users != null)
            {
                retval.Result = users;
                retval.Code = 1;
                retval.Message = "Thành công";
            }
            else
            {
                retval.Code = 0;
                retval.Message = "Lấy danh sách người dùng thất bại";
                retval.Result = new List<UserInfo>();
            }
            
            return retval;
        }
    }
}
