using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace S2TAnalytics.Common.Utilities
{
  public  class PasswordHasher
    {
        private int saltSize = 4;

        public int SaltSize
        {
            get { return saltSize; }
            set { saltSize = value; }
        }

        public  string PasswordEncrypt(string originalPassword)
        {
            // Convert the string original value to a byte array
            var originalData = Encoding.UTF8.GetBytes(originalPassword);

            // Create a 4-byte salt using a cryptographically secure random number generator
            var saltData = new byte[saltSize];
            var rng = new RNGCryptoServiceProvider();
            rng.GetNonZeroBytes(saltData);

            // Append the salt to the end of the original
            var saltedPasswordData = new byte[originalData.Length + saltData.Length];
            Array.Copy(originalData, 0, saltedPasswordData, 0, originalData.Length);
            Array.Copy(saltData, 0, saltedPasswordData, originalData.Length, saltData.Length);

            var sha = new SHA1Managed();
            var hashData = sha.ComputeHash(saltedPasswordData);

            var hashSaltData = new byte[hashData.Length + saltData.Length];
            Array.Copy(hashData, 0, hashSaltData, 0, hashData.Length);
            Array.Copy(saltData, 0, hashSaltData, hashData.Length, saltData.Length);

            return Convert.ToBase64String(hashSaltData);
        }

        public bool CompareStringToHash(string password, string hashPassword)
        {
            var hashData = Convert.FromBase64String(hashPassword);
            // First, pluck the four-byte salt off of the end of the hash
            var saltData = new byte[SaltSize];
            Array.Copy(hashData, hashData.Length - saltData.Length, saltData, 0, saltData.Length);

            var passwordData = Encoding.UTF8.GetBytes(password);

            // Append the salt to the end of the original
            var saltedPasswordData = new byte[passwordData.Length + saltData.Length];
            Array.Copy(passwordData, 0, saltedPasswordData, 0, passwordData.Length);
            Array.Copy(saltData, 0, saltedPasswordData, passwordData.Length, saltData.Length);

            // Create a new SHA-1 instance and compute the hash 
            var sha = new SHA1Managed();
            var newHashData = sha.ComputeHash(saltedPasswordData);

            // Add salt bytes onto end of the original hash for storage
            var newHashSaltData = new byte[newHashData.Length + saltData.Length];
            Array.Copy(newHashData, 0, newHashSaltData, 0, newHashData.Length);
            Array.Copy(saltData, 0, newHashSaltData, newHashData.Length, saltData.Length);
            // Compare and return
            return (Convert.ToBase64String(hashData).Equals(Convert.ToBase64String(newHashSaltData)));
        }

    }
}
