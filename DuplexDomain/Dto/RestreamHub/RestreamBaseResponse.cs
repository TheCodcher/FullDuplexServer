using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DuplexDomain.Dto.RestreamHub
{
    public class RestreamBaseResponseDto
    {
        public bool ExceptionExists { get; set; }
        public string[] Exceptions { get; set; }
        public static T SetException<T>(params string[] exceptions) where T: RestreamBaseResponseDto, new()
        {
            return new T()
            {
                ExceptionExists = true,
                Exceptions = exceptions.Clone() as string[]
            };
        }
        public static RestreamBaseResponseDto SetException(params string[] exceptions) => SetException<RestreamBaseResponseDto>(exceptions);
    }
}
