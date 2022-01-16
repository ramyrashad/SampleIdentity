using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SampleIdentity.Core.Services.Account.Models
{
    public class RefreshTokenViewModel
    {
        public RefreshTokenViewModel(string userName, DateTime expiresUtc)
        {
            Username = userName;
            ExpiresUtc = expiresUtc;
        }
        /// <summary>
        /// 
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime ExpiresUtc { get; set; }
    }
}
