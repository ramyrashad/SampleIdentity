using SampleIdentity.Core.Services.Account.Misc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SampleIdentity.Core.Services.Account.Interfaces
{
    public interface IRoleService
    {
        Task<bool> IsLoggedInUserInAnyRoles(IEnumerable<SystemRoles> applicationRolesToCheck);
    }
}
