using SampleIdentity.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SampleIdentity.Core.Services.Account.Interfaces
{
    public interface IClientService
    {
        Task<Client> ValidatActiveClientAsync(string clientId);
    }
}
