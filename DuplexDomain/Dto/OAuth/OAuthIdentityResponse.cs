using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DuplexDomain.Dto.OAuth
{
    public class OAuthIdentityResponseDto
    {
        public TimeSpan DataLifeTime { get; set; }

        public UserIdentityDto Identity { get; set; }
    }

    public class UserIdentityDto
    {
        public string AuthenticationType { get; set; }

        public bool IsAuthenticated { get; set; }

        public string Name { get; set; }
    }
}
