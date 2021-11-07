using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DuplexDomain.Dto.RestreamHub
{
    public class RestreamGroupUsersDto : RestreamBaseResponseDto
    {
        public string[] GroupUsers { get; set; }
    }
}
