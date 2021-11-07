using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;

namespace FullDuplexServer.Abstractions
{
    public interface ITokenValidationService
    {
        Task<IIdentity> ValidateAsync(string token);
    }
}
