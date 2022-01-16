using SampleIdentity.Core.Entities.ApplicationUserAggregate;

namespace SampleIdentity.WebAPI.Providers.Interfaces
{
    public interface ITokenGeneratorProvider
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        Task<object> GenerateUserToken(ApplicationUser user, string clientId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="refreshTokenKey"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        Task<object> GenerateUserTokenByRefreshToken(string refreshTokenKey, string clientId);
    }
}
