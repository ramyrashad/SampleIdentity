using SampleIdentity.Core.Entities.ApplicationUserAggregate;
using SampleIdentity.Core.Services.Account.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SampleIdentity.Core.Services.Account.Interfaces
{
    public interface IUserAccountService
    {
        Task<ApplicationUser> AuthorizeUser(AutheticationBindingModel autheticationBindingModel);
        Task<ApplicationUser> FindNormalUserByUserName(string userName);
    }
}
