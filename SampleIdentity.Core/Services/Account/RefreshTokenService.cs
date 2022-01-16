using SampleIdentity.Core.Common.Utilities;
using SampleIdentity.Core.Entities;
using SampleIdentity.Core.Repositories;
using SampleIdentity.Core.Repositories.Base;
using SampleIdentity.Core.Repositories.Interfaces;
using SampleIdentity.Core.Services.Account.Interfaces;
using SampleIdentity.Core.Services.Account.Models;
using SampleIdentity.Core.Services.Account.Specifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SampleIdentity.Core.Services.Account
{
    public class RefreshTokenService : IRefreshTokenService
    {
        private readonly IRepository<RefreshToken> _refreshTokenRepository;
        private readonly IClientService _clientService;
        private readonly IUnitOfWork _unitOfWork;

        public RefreshTokenService(IRepository<RefreshToken> refreshTokenRepository,
            IClientService clientService,
            IUnitOfWork unitOfWork)
        {
            _refreshTokenRepository = refreshTokenRepository;
            _clientService = clientService;
            _unitOfWork = unitOfWork;
        }

        public async Task<string> GenerateRefreshTokenAsync(string username, string clientId)
        {
            var client = await _clientService.ValidatActiveClientAsync(clientId);
            var randomTicket = HashingUtility.GenerateRandomEncryptedTicket();

            var generatedRefreshToken = new RefreshToken(username, clientId, DateTime.UtcNow,
                DateTime.UtcNow.AddMinutes(client.RefreshTokenLifeTime), randomTicket);

            _refreshTokenRepository.Add(generatedRefreshToken);
            await _unitOfWork.CommitAsync();

            return generatedRefreshToken.ProtectedTicket;
        }

        public async Task<RefreshTokenViewModel> GetRefreshTokenByProtectedTicketAsync(string protectedTicket)
        {
            var refreshToken = await _refreshTokenRepository
                .GetFirstOrDefaultAsync(new RefreshTokenByTicketSpecifications(protectedTicket));
            if (refreshToken == null)
                return null;

            var refreshTokenViewModel = new RefreshTokenViewModel(refreshToken.UserName, refreshToken.ExpiresUtc);
            return refreshTokenViewModel;
        }

        public async Task DeleteRefreshTokenByProtectedKeyAsync(string protectedTicket)
        {
            var refreshToken = await _refreshTokenRepository
                .GetFirstOrDefaultAsync(new RefreshTokenByTicketSpecifications(protectedTicket));
            if (refreshToken == null)
                return;

            _refreshTokenRepository.Delete(refreshToken);
            await _unitOfWork.CommitAsync();
        }
    }
}
