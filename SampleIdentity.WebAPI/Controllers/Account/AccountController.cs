using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SampleIdentity.Core.Common.Configurations;
using SampleIdentity.Core.Services.Account.Interfaces;
using SampleIdentity.Core.Services.Account.Models;
using SampleIdentity.WebAPI.Filters;
using SampleIdentity.WebAPI.Providers.Interfaces;
using System.Net;

namespace SampleIdentity.WebAPI.Controllers.Account
{
    /// <summary>
	/// 
	/// </summary>
	[Route("api/Account")]
    [Produces("application/json")]
    [ApiController]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    public class AccountController : ControllerBase
    {
        private readonly IUserAccountService _accountService;
        private readonly IOAuthAuthorizationProvider _oAuthAuthorizationProvider;
        private readonly ConfigurationsManager _configurationsManager;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="accountService"></param>
        /// <param name="oAuthAuthorizationProvider"></param>
        /// <param name="configurationsManager"></param>
        public AccountController(IUserAccountService accountService,
            IOAuthAuthorizationProvider oAuthAuthorizationProvider,
            ConfigurationsManager configurationsManager)
        {
            _accountService = accountService;
            _oAuthAuthorizationProvider = oAuthAuthorizationProvider;
            _configurationsManager = configurationsManager;
        }

        /// <summary>
        /// Authenticate the user by (username and password) and generate a JWT token for this user.
        /// </summary>
        /// <param name="loginBindingModel"></param>
        /// <returns></returns>
        [HttpPost("token")]
        [AllowAnonymous]
        [ApiExplorerSettings(IgnoreApi = true)]
        [ServiceFilter(typeof(ClientAuthenticationFilterAttribute))]
        public async Task<IActionResult> Token(LoginBindingModel loginBindingModel)
        {
            var userToken = await _oAuthAuthorizationProvider.GrantCredentials(Request, loginBindingModel);
            return Ok(userToken);
        }

        /// <summary>
        /// Get a new JWT token by using refresh token.
        /// </summary>
        /// <param name="refresh_token"></param>
        /// <returns></returns>
        [HttpPost("refreshtoken")]
        [AllowAnonymous]
        [ApiExplorerSettings(IgnoreApi = true)]
        [ServiceFilter(typeof(ClientAuthenticationFilterAttribute))]
        //[RestrictedIPFilter(isRestrictedLogin: true)]
        public async Task<IActionResult> RefreshToken(string refresh_token)
        {
            var userToken = await _oAuthAuthorizationProvider.GrantRefreshToken(Request, refresh_token);
            return Ok(userToken);
        }
       
    }
}


