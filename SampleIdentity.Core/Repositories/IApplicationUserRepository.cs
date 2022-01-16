using SampleIdentity.Core.Entities.ApplicationUserAggregate;
using SampleIdentity.Core.Repositories.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SampleIdentity.Core.Repositories
{
    public interface IApplicationUserRepository : IRepository<ApplicationUser>
    {
    }
}
