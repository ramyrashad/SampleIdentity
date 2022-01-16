using SampleIdentity.Core.Entities;
using SampleIdentity.Core.Repositories;
using SampleIdentity.Core.Repositories.Base;
using SampleIdentity.Core.Repositories.Interfaces;
using SampleIdentity.Core.Services.Account.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SampleIdentity.Core.Services.Account
{
    public class ClientService : IClientService
    {
        private readonly IRepository<Client> _clientRepository;
        private readonly IUnitOfWork _unitOfWork;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="clientRepository"></param>
        /// <param name="unitOfWork"></param>
        public ClientService(IRepository<Client> clientRepository, IUnitOfWork unitOfWork)
        {
            _clientRepository = clientRepository;
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Validate the client is active and then return it
        /// </summary>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<Client> ValidatActiveClientAsync(string clientId)
        {
            var client = await _clientRepository.GetByIdAsync(clientId);
            if (client == null)
                throw new Exception("ClientNotActive");

            if (client.IsActive == false)
                throw new Exception("ClientNotActive");

            return client;
        }
    }
}
