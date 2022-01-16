using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SampleIdentity.Core.Services.Account.Models
{
    public class LoginBindingModel
    {
        [Required]
        public string UserName { get; set; }

        [Required]
        public string Password { get; set; }

    }

    public class AutheticationBindingModel
    {
        public AutheticationBindingModel(string userName, string password,
            string clientId)
        {
            UserName = userName ?? throw new ArgumentNullException(nameof(userName));
            Password = password ?? throw new ArgumentNullException(nameof(password));
            ClientId = clientId;
        }

        public string UserName { get; set; }

        public string Password { get; set; }

        public string ClientId { get; set; }
    }

}
