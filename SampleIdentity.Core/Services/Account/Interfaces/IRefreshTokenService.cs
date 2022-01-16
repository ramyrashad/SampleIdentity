using SampleIdentity.Core.Services.Account.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SampleIdentity.Core.Services.Account.Interfaces
{
    public interface IRefreshTokenService
    {
        Task<string> GenerateRefreshTokenAsync(string username, string clientId);

        Task<RefreshTokenViewModel> GetRefreshTokenByProtectedTicketAsync(string protectedTicket);

        Task DeleteRefreshTokenByProtectedKeyAsync(string refreshToken);
    }
}
