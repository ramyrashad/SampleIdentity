using SampleIdentity.Core.Entities.ApplicationUserAggregate;
using SampleIdentity.Core.Repositories;
using SampleIdentity.Infrastructure.Data.Context;
using SampleIdentity.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SampleIdentity.Infrastructure.Data.Repositories
{
    public class ApplicationUserRepository : RepositoryBase<ApplicationUser>, IApplicationUserRepository
    {
        public ApplicationUserRepository(ApplicationDbContext dbContext) : base(dbContext)
        {

        }
    }
}
