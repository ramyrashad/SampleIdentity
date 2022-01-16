using Microsoft.Extensions.Primitives;
using System.Text;

namespace SampleIdentity.WebAPI.Filters
{
    public static class HttpRequestHeaderHelper
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        public static string GetClientIdFromHeader(HttpRequest request)
        {
            request.Headers.TryGetValue("Authorization", out StringValues authHeader);
            ExtractClientIdAndSecretFromHeader(authHeader, out string clientId, out string clientSecret);

            return clientId;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="clientId"></param>
        /// <param name="clientSecret"></param>
        public static void GetClientIdAndSecretFromHeader(HttpRequest request,
            out string clientId, out string clientSecret)
        {
            request.Headers.TryGetValue("Authorization", out StringValues authHeader);
            ExtractClientIdAndSecretFromHeader(authHeader, out clientId, out clientSecret);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="authHeader"></param>
        /// <param name="clientId"></param>
        /// <param name="clientSecret"></param>
        public static void ExtractClientIdAndSecretFromHeader(string authHeader,
            out string clientId, out string clientSecret)
        {
            clientId = string.Empty;
            clientSecret = string.Empty;

            var isClientInfoExist = string.IsNullOrEmpty(authHeader) == false &&
                authHeader.ToString().StartsWith("Basic");
            if (isClientInfoExist)
            {
                var encodedClientAndSecret = authHeader.Substring("Basic ".Length).Trim();
                var decodedClientAndSecret = Convert.FromBase64String(encodedClientAndSecret);
                var clientAndSecret = Encoding.GetEncoding("iso-8859-1").GetString(decodedClientAndSecret);
                var clientAndSecretSplitted = clientAndSecret.Split(':').ToList();

                if (clientAndSecretSplitted.Any())
                {
                    clientId = clientAndSecretSplitted[0];
                    clientSecret = clientAndSecretSplitted[1];
                }
            }
        }
    }
}
