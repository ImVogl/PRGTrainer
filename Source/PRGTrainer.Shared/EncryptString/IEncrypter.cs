namespace PRGTrainer.Shared.EncryptString
{
    /// <summary>
    /// Интерфейс шифровальщика.
    /// </summary>
    public interface IEncrypter
    {
        /// <summary>
        /// Шифрование стоки.
        /// </summary>
        /// <param name="value">Строка, которую предстоит зашифровать.</param>
        /// <returns>Зашифрованная строка.</returns>
        string Encrypt(string value);
    }
}