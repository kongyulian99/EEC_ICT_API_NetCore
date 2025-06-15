using System;
using System.Collections.Generic;
using System.Text;

namespace ApiService.Entity
{
    public class UserRoleInfo
    {
        public Guid ID { get; set; }
        public string cUserName { get; set; }
        public string cUserFullName { get; set; }
        public short cStatus { get; set; }
        public List<RoleExInfo> roles { get; set; }

    }
    public class RoleExInfo
    {
        public Guid fkRoleId { get; set; }
        public string cRoleCode { get; set; }
        public string cRoleName { get; set; }
        public List<PermissionExInfo> permissions { get; set; }
    }

    public class PermissionExInfo
    {
        public Guid fkPermissionId { get; set; }
        public string cPermissionCode { get; set; }
        public string cPermissionName { get; set; }
    }
    
}
