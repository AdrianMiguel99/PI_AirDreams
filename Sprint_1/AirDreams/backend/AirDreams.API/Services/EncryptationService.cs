using System;
using System.Security.Cryptography;
using System.Text;

namespace AirDreams.API.Services
{
    public class EncryptionService : IEncryptionService
    {
        private readonly string _key;
        
        public EncryptionService(IConfiguration configuration)
        {
            _key = configuration["Encryption:Key"] ?? "AirDreamsSecretKey1234567890123456"; 
        }
        
        public string Encrypt(string plainText)
        {
            using var aes = Aes.Create();
            aes.Key = Encoding.UTF8.GetBytes(_key);
            aes.IV = new byte[16]; 
            
            var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
            var plainBytes = Encoding.UTF8.GetBytes(plainText);
            var cipherBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);
            
            return Convert.ToBase64String(cipherBytes);
        }
        
        public string Decrypt(string cipherText)
        {
            using var aes = Aes.Create();
            aes.Key = Encoding.UTF8.GetBytes(_key);
            aes.IV = new byte[16];
            
            var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
            var cipherBytes = Convert.FromBase64String(cipherText);
            var plainBytes = decryptor.TransformFinalBlock(cipherBytes, 0, cipherBytes.Length);
            
            return Encoding.UTF8.GetString(plainBytes);
        }
    }
}