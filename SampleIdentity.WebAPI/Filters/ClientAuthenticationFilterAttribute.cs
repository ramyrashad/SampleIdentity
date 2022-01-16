using Microsoft.AspNetCore.Mvc.Filters;
using SampleIdentity.Core.Repositories;
using SampleIdentity.Core.Services.Account.Interfaces;

namespace SampleIdentity.WebAPI.Filters
{
    public class ClientAuthenticationFilterAttribute : ActionFilterAttribute
    {
        private readonly IClientService _clientService;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="clientService"></param>
        public ClientAuthenticationFilterAttribute(IClientService clientService)
        {
            _clientService = clientService;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="actionContext"></param>
        public override void OnActionExecuting(ActionExecutingContext actionContext)
        {
            return;
            //actionContext.HttpContext.Request.Headers.TryGetValue("Authorization", out StringValues authHeader);

            //HttpRequestHeaderHelper.ExtractClientIdAndSecretFromHeader(authHeader, out string clientId, out string clientSecret);

            //_clientService.ValidateClientAndSecret(clientId, clientSecret);
        }
    }
}
