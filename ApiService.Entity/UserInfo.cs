using System;
using System.Collections.Generic;
using System.Text;

namespace ApiService.Entity
{
    public class UserInfo
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password_Hash { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string Full_Name { get; set; }
        public bool Is_Admin { get; set; }
        public bool Is_Active { get; set; }
        public DateTime Created_At { get; set; }
        public DateTime Updated_At { get; set; }
        public DateTime? Last_Login { get; set; }
    }
}
