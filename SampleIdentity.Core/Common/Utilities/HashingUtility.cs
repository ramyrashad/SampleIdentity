using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SampleIdentity.Core.Common.Utilities
{
    public class HashingUtility
    {
        public static string GetHash(string input)
        {
            var hashAlgorithm = new SHA256CryptoServiceProvider();
            var byteValue = Encoding.UTF8.GetBytes(input);
            var byteHash = hashAlgorithm.ComputeHash(byteValue);

            return Convert.ToBase64String(byteHash);
        }

        public static string GenerateRandomEncryptedTicket()
        {
            //var hashAlgorithm = new SHA256CryptoServiceProvider();
            //var byteValue = Encoding.UTF8.GetBytes(Guid.NewGuid().ToString("n"));
            //var byteHash = hashAlgorithm.ComputeHash(byteValue);
            //var randomTicket = Convert.ToBase64String(byteHash);
            //randomTicket = HttpUtility.UrlEncode(randomTicket);

            var randomTicket = $"{Guid.NewGuid()}{Guid.NewGuid()}";
            randomTicket = randomTicket.Replace("-", "");
            return randomTicket;
        }

        public static string GenerateRandomPassword()
        {
            var randomChars = new[] { "abcdefghijkmnopqrstuvwxyz", "ABCDEFGHIJKLMNOPQRSTUVWXYZ", "0123456789" };

            var rand = new Random(Environment.TickCount);
            var chars = new List<char>();

            chars.Insert(rand.Next(0, chars.Count), randomChars[0][rand.Next(0, randomChars[0].Length)]);
            chars.Insert(rand.Next(0, chars.Count), randomChars[1][rand.Next(0, randomChars[1].Length)]);
            chars.Insert(rand.Next(0, chars.Count), randomChars[2][rand.Next(0, randomChars[2].Length)]);

            for (var i = chars.Count; i <= 8; i++)
            {
                var rcs = randomChars[rand.Next(0, randomChars.Length)];
                chars.Insert(rand.Next(0, chars.Count), rcs[rand.Next(0, rcs.Length)]);
            }

            return new string(chars.ToArray());
        }

        internal static string GenerateSalt()
{
            var rngCsp = new RNGCryptoServiceProvider();
            var bytes = new byte[8];
            rngCsp.GetBytes(bytes);
            return Convert.ToBase64String(bytes);
        }

        public static string HashPassword(string password, out string salt)
        {
            salt = GenerateSalt();
            return GetMd5Hash(password + salt);
        }

        public static string HashPassword(string password, string salt)
        {
            return GetMd5Hash(password + salt);
        }


        #region Private Methods

        private static string GetMd5Hash(string input)
        {
            MD5 md5Hash = MD5.Create();
            // Convert the input string to a byte array and compute the hash. 
            byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

            // Create a new Stringbuilder to collect the bytes 
            // and create a string.
            StringBuilder sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data  
            // and format each one as a hexadecimal string. 
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            // Return the hexadecimal string. 
            return sBuilder.ToString();
        }
        #endregion
    }
}
