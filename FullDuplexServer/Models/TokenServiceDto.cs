using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FullDuplexServer.Models
{
    public class TokenServiceDto
    {
        public TimeSpan DataLifeTime { get; set; }
        public UserIdentity Identity { get; set; }
    }
}
