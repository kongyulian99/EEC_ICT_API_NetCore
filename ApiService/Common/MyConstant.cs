using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiService.Common
{
    public class MyConstant
    {
        public const string UserRoleCacheKey = "USER_ROLE_";
        public const string UserInfoCacheKey = "USER_INFO_";
        public const string MappingCommandCacheKey = "LIST_MAPPING_COMMAND";

        public enum CommandCode
        {
            NOTE_EXIST_COMMAND,
            //----

            USER_LIST,
            USER_FORTAG_LIST,
            USER_ADD,
            USER_UPDATE,
            USER_GET_INFO,
            USER_DELETE,
            USER_GET_INFOGROUP,
            USER_ADD_INFOGROUP,
            USER_UPDATE_INFOGROUP,
            USER_DELETE_INFOGROUP,
            USER_RESET_PASSWORD,
            USER_CHANGE_PASSWORD,
            USER_LOGOUT,
            USER_FORCE_LOGOUT,
            USER_GET_SIDEBAR,

            ROLE_LIST,
            ROLE_ADD,
            ROLE_UPDATE,
            ROLE_GET_INFO,
            ROLE_DELETE,
            ROLE_GET_PERMISSIONSINROLE,
            ROLE_ADD_PERMISSIONSINROLE,
            ROLE_DELETE_PERMISSIONSINROLE,
            ROLE_LIST_EXPERMISSIONSINROLE,
        }
    }
}
