using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DuplexDomain.Dto.RestreamHub
{
    public static class RestreamHubDefaults
    {
        public const string CLIENT_DATA_METHOD = "Recieve";
        public const string CLIENT_EXCEPTION_METHOD = "Exception";
        public const string CLIENT_GROUP_USERS_METHOD = "UpdateUsers";
        public const string CLIENT_GROUP_CODE_METHOD = "SetGroupCode";

        public const string SERVER_DATA_METHOD = "Recieve";
        public const string SERVER_JOIN_METHOD = "Join";
        public const string SERVER_CREATE_METHOD = "Create";
    }
}
