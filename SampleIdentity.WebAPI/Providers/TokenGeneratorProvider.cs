using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SampleIdentity.Core.Common.Configurations;
using SampleIdentity.Core.Entities.ApplicationUserAggregate;
using SampleIdentity.Core.Repositories;
using SampleIdentity.Core.Services.Account.Interfaces;
using SampleIdentity.WebAPI.Providers.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SampleIdentity.WebAPI.Providers
{
    public class TokenGeneratorProvider : ITokenGeneratorProvider
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ConfigurationsManager _configuration;
        private readonly IRefreshTokenService _refreshTokenService;
        private readonly IUserAccountService _accountService;
        private readonly IMediator _mediator;
        private readonly IHttpContextAccessor _context;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userManager"></param>
        /// <param name="configuration"></param>
        /// <param name="refreshTokenService"></param>
        /// <param name="accountService"></param>
        /// <param name="enterpriseOrganizationService"></param>
        /// <param name="clientPlanSubscriptionService"></param>
        /// <param name="mediator"></param>
        /// <param name="context"></param>
        public TokenGeneratorProvider(
          UserManager<ApplicationUser> userManager,
          ConfigurationsManager configuration,
          IRefreshTokenService refreshTokenService,
          IUserAccountService accountService,
          IMediator mediator,
          IHttpContextAccessor context)
        {
            _userManager = userManager;
            _configuration = configuration;
            _refreshTokenService = refreshTokenService;
            _accountService = accountService;
            _mediator = mediator;
            _context = context;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <param name="clientId"></param>
        /// <param name="responseType"></param>
        /// <returns></returns>
        public async Task<object> GenerateUserToken(ApplicationUser user, string clientId)
        {
            var utcNow = DateTime.UtcNow;
            var claims = new Claim[]
            {
                        new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                        new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName),
                        new Claim(JwtRegisteredClaimNames.Email, user.Email),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                        new Claim(JwtRegisteredClaimNames.Iat, utcNow.ToString()),
                        new Claim(nameof(user.EmailConfirmed), user.EmailConfirmed.ToString()),

            };

            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.Tokens.SecretKey));
            var signingCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);
            var jwt = new JwtSecurityToken(
                    signingCredentials: signingCredentials,
                    claims: claims,
                    notBefore: utcNow,
                    expires: utcNow.AddSeconds(_configuration.Tokens.Lifetime),
                    audience: _configuration.Tokens.Issuer,
                    issuer: _configuration.Tokens.Issuer
                );



            return await BuildTokenObject(user, jwt, utcNow, clientId);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="refreshTokenKey"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<object> GenerateUserTokenByRefreshToken(string refreshTokenKey, string clientId)
        {
            var refreshToken = await _refreshTokenService.GetRefreshTokenByProtectedTicketAsync(refreshTokenKey);
            if (refreshToken == null)
                throw new SecurityTokenException("Invalid refresh token.");

            if (refreshToken.ExpiresUtc < DateTime.UtcNow)
                throw new SecurityTokenException("Refresh token expired.");

            var user = await _accountService.FindNormalUserByUserName(refreshToken.Username);
            if (user == null)
                throw new SecurityTokenException("User is not exists.");

            await _refreshTokenService.DeleteRefreshTokenByProtectedKeyAsync(refreshTokenKey);

            return await GenerateUserToken(user, clientId);
        }

        #region Private Methods

        private async Task<object> BuildTokenObject(ApplicationUser user, JwtSecurityToken jwt,
            DateTime utcNow, string clientId)
        {
            var roles = await _userManager.GetRolesAsync(user);
            var refreshToken = await _refreshTokenService.GenerateRefreshTokenAsync(user.UserName, clientId);


            return new
            {
                access_token = new JwtSecurityTokenHandler().WriteToken(jwt),
                token_type = "bearer",
                expires_in = _configuration.Tokens.Lifetime,
                refresh_token = refreshToken,
                client_id = clientId,
                userName = user.UserName,
                name = user.FirstName + " " + user.LastName,
                role = roles.ToList(),
                firstName = user.FirstName,
                lastName = user.LastName,
                email = user.Email,
                issued = utcNow,
                expires = utcNow.AddSeconds(_configuration.Tokens.Lifetime)
            };
        }

        #endregion
    }
}
