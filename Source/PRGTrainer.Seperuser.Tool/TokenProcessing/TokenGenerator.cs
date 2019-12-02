namespace PRGTrainer.SuperUser.Tool.TokenProcessing
{
    using System;
    using System.Configuration;
    using System.Data.SqlClient;
    using Shared.EncryptString;

    /// <summary>
    /// Генератор токенов.
    /// </summary>
    public class TokenGenerator
    {
        /// <summary>
        /// Символы, из которых будет формироваться токен.
        /// </summary>
        private const string Chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789[]{}!@#$%^&*?";

        /// <summary>
        /// Шифровальщик.
        /// </summary>
        private readonly IEncrypter _encrypter;

        /// <summary>
        /// Подключение к серверу.
        /// </summary>
        private readonly SqlConnection _connection;

        /// <summary>
        /// Рандомайзер.
        /// </summary>
        private readonly Random _random;

        /// <summary>
        /// Инициализирует экземпляр <see cref="TokenGenerator"/>
        /// </summary>
        public TokenGenerator()
        {
            _random = new Random();
            _encrypter = new Encrypter();
            _connection = new SqlConnection(ConfigurationManager.ConnectionStrings[@"Main"].ConnectionString);
        }

        /// <summary>
        /// Генерация токена и сохранение его в базе данных.
        /// </summary>
        /// <param name="uStatAvailable">Разрешено читать статистику пользователей.</param>
        /// <param name="qStatAvailable">Разрешено читать статистику по вопросам.</param>
        /// <returns>Токен.</returns>
        public string Generate(bool uStatAvailable, bool qStatAvailable)
        {
            var token = CreateToken();
            SaveToken(token, uStatAvailable, qStatAvailable);
            return token;
        }

        /// <summary>
        /// Генерация токена.
        /// </summary>
        /// <returns>Токен.</returns>
        private string CreateToken()
        {
            var stringChars = new char[10];
            for (var i = 0; i < stringChars.Length; i++)
                stringChars[i] = Chars[_random.Next(Chars.Length)];
            
            return new string(stringChars);
        }

        /// <summary>
        /// Сохранение токена.
        /// </summary>
        /// <param name="token">Токен.</param>
        /// <param name="userStat">Разрешено читать статистику пользователей.</param>
        /// <param name="questionStat">Разрешено читать статистику по вопросам.</param>
        private void SaveToken(string token, bool userStat, bool questionStat)
        {
            var uStat = userStat ? 1 : 0;
            var qStat = questionStat ? 1 : 0;
            var query = $"INSERT INTO dbo.Tokens(Token, UserStat, QuestionsStat) VALUES('{_encrypter.Encrypt(token)}', {uStat}, {qStat})";
            try
            {
                _connection.Open();
                using (var command = new SqlCommand(query, _connection))
                {
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
            finally
            {
                _connection.Close();
            }
        }
    }
}