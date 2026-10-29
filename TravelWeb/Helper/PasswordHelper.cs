using Microsoft.AspNetCore.Identity;
using System;
using System.Security.Cryptography;
using System.Text;

namespace TravelWeb.Helpers
{
    public static class PasswordHelper
    {
        public static string Hash(string password)
        {
            if (string.IsNullOrEmpty(password)) return "";
            using (var sha256 = SHA256.Create())
            {
                var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(bytes);
            }
        }
        public static bool Verify(string inputPassword, string storedHash)
        {
            if (string.IsNullOrEmpty(inputPassword) || string.IsNullOrEmpty(storedHash))
                return false;

            // Hash the input password nd see if it matches the DB
            string hashedInput = Hash(inputPassword);
            return hashedInput == storedHash;
        }
    }
}
