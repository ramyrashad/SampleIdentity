using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SampleIdentity.Core.Entities
{
    public class RefreshToken
    {
        #region Constructors
        private RefreshToken()
        {
        }

        public RefreshToken(string userName, string clientId, DateTime issuedUtc,
            DateTime expiresUtc, string protectedTicket)
        {
            Id = Guid.NewGuid().ToString();
            UserName = userName;
            ClientId = clientId;
            IssuedUtc = issuedUtc;
            ExpiresUtc = expiresUtc;
            ProtectedTicket = protectedTicket;
        }

        #endregion

        public string Id { get; private set; }

        [Required]
        [MaxLength(50)]
        public string UserName { get; private set; }

        [Required]
        [MaxLength(50)]
        public string ClientId { get; private set; }

        [Required]
        public DateTime IssuedUtc { get; set; }

        [Required]
        public DateTime ExpiresUtc { get; private set; }

        [Required]
        public string ProtectedTicket { get; private set; }
    }
}
