using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SampleIdentity.Core.Common.Configurations
{
    public class ConfigurationsManager
    {
        public TokenConfiguration Tokens { get; set; } = new TokenConfiguration();
    }

    public class TokenConfiguration
    {
        public int TokenExpireMinutes { get; set; }

        public int RefreshTokenExpireMinutes { get; set; }

        public int SocialNetworkTokenExpireMinutes { get; set; }

        public string SecretKey { get; set; }

        public string Issuer { get; set; }

        public string Audience { get; set; }

        public string TokenPath { get; set; }

        public string CookieName { get; set; }

        public int Lifetime { get; set; }
    }
}
