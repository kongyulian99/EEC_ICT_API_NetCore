using ApiService.Entity;
using System;
using System.Collections.Generic;
using System.Data;

namespace ApiService.Interface
{
    public interface IUser
    {
        DbReturnInfo<int> CreateUser(UserInfo user);
        DbReturnInfo<UserInfo> GetUserById(int id);
        DbReturnInfo<UserInfo> GetUserByUsername(string username);
        DbReturnInfo<int> UpdateUser(UserInfo user);
        DbReturnInfo<int> UpdateUserPassword(int id, string newPasswordHash);
        DbReturnInfo<int> DeleteUser(int id);
        DbReturnInfo<int> UpdateLastLogin(int id);
        DbReturnInfo<List<UserInfo>> GetAllUsers();
    }
}
