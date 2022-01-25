using AutoMapper;
using Microsoft.AspNetCore.Identity;
using SampleIdentity.Core.Entities.ApplicationUserAggregate;
using SampleIdentity.Core.Repositories.Base;
using SampleIdentity.Core.Services.Account.Interfaces;
using SampleIdentity.Core.Services.Account.Misc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SampleIdentity.Core.Services.Account
{
    public class RoleService : IRoleService
    {
        private readonly IRepository<ApplicationRole> _applicationRoleRepository;
        private readonly IRepository<ApplicationUser> _applicationUserRepository;
        private readonly IUserAccountService _userAccountService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;

        public RoleService(IRepository<ApplicationRole> applicationRoleRepository,
            UserManager<ApplicationUser> userManager,
            IUserAccountService userAccountService,
            IMapper mapper,
            IRepository<ApplicationUser> applicationUserRepository)
        {
            _applicationRoleRepository = applicationRoleRepository;
            _userAccountService = userAccountService;
            _userManager = userManager;
            _mapper = mapper;
            _applicationUserRepository = applicationUserRepository;


        }

        public async Task<bool> IsLoggedInUserInAnyRoles(IEnumerable<SystemRoles> applicationRolesToCheck)
        {
            var loggedInUser = await _userAccountService.GetLoggedInUser();

            var isUserInSuperAdminRole = await IsUserSuperAdministrator(loggedInUser);
            if (isUserInSuperAdminRole)
                return true;

            var userRoles = await _userManager.GetRolesAsync(loggedInUser);
            var isRoleExist = applicationRolesToCheck.Any(c => userRoles.Any(r => string.Equals(r, c.ToString(), StringComparison.InvariantCultureIgnoreCase)));
            if (isRoleExist)
                return true;

            return false;
        }

        #region Private Methods

        public async Task<bool> IsUserSuperAdministrator(ApplicationUser user)
        {
            var superAdministratorRoleName = Enum.GetName(typeof(SystemRoles), SystemRoles.SuperAdministrator);
            var isUserInSuperAdminRole = await _userManager.IsInRoleAsync(user, superAdministratorRoleName);
            return isUserInSuperAdminRole;
        }

        #endregion
    }
}
