using Microsoft.AspNetCore.Identity;
using SampleIdentity.Core.Common.Configurations;
using SampleIdentity.Core.Entities.ApplicationUserAggregate;
using SampleIdentity.Core.Repositories;
using SampleIdentity.Core.Services.Account.Interfaces;
using SampleIdentity.Core.Services.Account.Models;
using SampleIdentity.WebAPI.Filters;
using SampleIdentity.WebAPI.Providers.Interfaces;

namespace SampleIdentity.WebAPI.Providers
{
    public class OAuthAuthorizationProvider : IOAuthAuthorizationProvider
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ConfigurationsManager _configuration;
        private readonly IRefreshTokenService _refreshTokenService;
        private readonly IClientService _clientService;
        private readonly IUserAccountService _accountService;
        private readonly ITokenGeneratorProvider _tokenGeneratorProvider;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userManager"></param>
        /// <param name="configuration"></param>
        /// <param name="refreshTokenService"></param>
        /// <param name="clientService"></param>
        /// <param name="accountService"></param>
        /// <param name="tokenGeneratorProvider"></param>
        public OAuthAuthorizationProvider(
          UserManager<ApplicationUser> userManager,
          ConfigurationsManager configuration,
          IRefreshTokenService refreshTokenService,
          IClientService clientService,
          IUserAccountService accountService,
          ITokenGeneratorProvider tokenGeneratorProvider)
        {
            _userManager = userManager;
            _configuration = configuration;
            _refreshTokenService = refreshTokenService;
            _clientService = clientService;
            _accountService = accountService;
            _tokenGeneratorProvider = tokenGeneratorProvider;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="loginBindingModel"></param>

        /// <returns></returns>
        public async Task<object> GrantCredentials(HttpRequest request, LoginBindingModel loginBindingModel)
        {
            var clientId = HttpRequestHeaderHelper.GetClientIdFromHeader(request);
            var authorizeRequest = new AutheticationBindingModel(loginBindingModel.UserName,
                loginBindingModel.Password,
                clientId);

            var loggedInUser = await _accountService.AuthorizeUser(authorizeRequest);

            var userToken = await _tokenGeneratorProvider.GenerateUserToken(loggedInUser, clientId);
            return userToken;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="refreshToken"></param>
        /// <returns></returns>
        public async Task<object> GrantRefreshToken(HttpRequest request, string refreshToken)
        {
            var clientId = HttpRequestHeaderHelper.GetClientIdFromHeader(request);
            return await _tokenGeneratorProvider.GenerateUserTokenByRefreshToken(refreshToken, clientId);
        }
    }
}
