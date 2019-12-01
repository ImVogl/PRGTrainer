namespace PRGTrainer.Shared.EncryptString
{
    using System;
    using System.Security.Cryptography;
    using System.Text;

    /// <summary>
    /// Шифровалищик.
    /// </summary>
    public class Encrypter : IEncrypter
    {
        /// <inheritdoc />
        public string Encrypt(string value)
        {
            using (var provider = new MD5CryptoServiceProvider())
            {
                var encoding = new UTF8Encoding();
                var data = provider.ComputeHash(encoding.GetBytes(value));
                return Convert.ToBase64String(data);
            }
        }
    }
}
