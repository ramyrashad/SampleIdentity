using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SampleIdentity.Core.Services.Account.Interfaces;
using SampleIdentity.Core.Services.Account.Misc;

namespace SampleIdentity.WebAPI.Filters
{
    public class AuthenticationFilterAttribute : AuthorizeAttribute, IAuthorizationFilter
    {
        /// <summary>
        ///  
        /// </summary>
        private readonly List<SystemRoles> _applicationRoles;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="requiredApplicationRole"></param>
        /// <param name="applicationRoles"></param>
        public AuthenticationFilterAttribute(params SystemRoles[] applicationRoles)
        {
            _applicationRoles = applicationRoles.ToList();

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            if (context.HttpContext.User.Identity.IsAuthenticated == false)
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            var roleService = (IRoleService)context.HttpContext.RequestServices.GetService(typeof(IRoleService));
            var isUserInRole = roleService.IsLoggedInUserInAnyRoles(_applicationRoles).Result;
            if (isUserInRole == false)
            {
                context.Result = new UnauthorizedResult();
                return;
            }
        }
    }

}
