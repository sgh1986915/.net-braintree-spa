using System;
using System.Security.Cryptography;
using System.Text;

namespace MySitterHub.Model.Misc
{
    public class StringHasher
    {

        public static string GetHashString(string inputString)
        {
            if (inputString == null)
                return null;

            const string salt = "47lm5&63eliZ--#4";
            
            var sb = new StringBuilder();
            foreach (byte b in GetHash(inputString + salt))
                sb.Append(b.ToString("X2"));

            return sb.ToString();
        }

        public static byte[] GetHash(string inputString)
        {
            HashAlgorithm algorithm = MD5.Create(); //or use SHA1.Create();
            return algorithm.ComputeHash(Encoding.UTF8.GetBytes(inputString));
        }

        public static Guid GenerateHash()
        {
            return Guid.NewGuid();
        }

    }
}