namespace PRGTrainer.Core.AdminHandler
{
    using System;
    using System.Collections.Generic;
    using System.Data.SqlClient;
    using System.Threading.Tasks;
    using JetBrains.Annotations;
    using log4net;
    using ResultFileGenerator;

    /// <summary>
    /// Обработчик администрирования.
    /// </summary>
    public class AdminHandler : IAdminHandler
    {
        #region Private fields

        /// <summary>
        /// Таблица с администраторами.
        /// </summary>
        private const string AdminsTable = @"Admins";

        /// <summary>
        /// Таблица с токенами.
        /// </summary>
        private const string TokensTable = @"Tokens";

        /// <summary>
        /// Генератор результатов пользователей.
        /// </summary>
        private readonly IResultFileGenerator _resultFileGenerator;

        /// <summary>
        /// Соединение с сервером.
        /// </summary>
        private readonly SqlConnection _connection;

        #endregion

        /// <summary>
        /// Инициализирует экземпляр <see cref="AdminHandler"/>
        /// </summary>
        /// <param name="resultFileGenerator">Генератор результатов пользователей.</param>
        private AdminHandler([NotNull]IResultFileGenerator resultFileGenerator)
        {
            _resultFileGenerator = resultFileGenerator;
            var connectionString = System.Configuration.ConfigurationManager.ConnectionStrings[@"UserStatistics"].ConnectionString;
            _connection = new SqlConnection(connectionString);
            Logger = LogManager.GetLogger(typeof(AdminHandler));
        }

        /// <inheritdoc />
        public StatisticOutputFileType OutputFileType { private get; set; }

        /// <summary>
        /// Логгер.
        /// </summary>
        private ILog Logger { get; }

        /// <inheritdoc />
        public async Task<bool> TryAddNewAdmin(string token)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public async Task<bool> IsUserAdmin(int identifier)
        {
            var query = $"SELECT Count(*) FROM dbo.{AdminsTable} WHERE UserId = '{identifier}'";
            try
            {
                await _connection.OpenAsync().ConfigureAwait(false);
                using (var command = new SqlCommand(query, _connection))
                {
                    var reader = await command.ExecuteReaderAsync().ConfigureAwait(false);
                    await reader.ReadAsync().ConfigureAwait(false);
                    return reader.GetInt32(0) == 0; 
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                if (Logger.IsErrorEnabled)
                    Logger.Error(@"Не удалось проверить является ли пользователь администратором!", exception);

                return false;
            }
            finally
            {
                _connection.Close();
            }
        }

        /// <inheritdoc />
        public async Task<string> GetStatisticForUsers(ICollection<string> users, DateTime startDate, int identifier)
        {
            if (OutputFileType.Equals(StatisticOutputFileType.Image))
            { }

            if(OutputFileType.Equals(StatisticOutputFileType.Text))
            { }
            
            throw new ArgumentOutOfRangeException("", OutputFileType, @"Неизвестный тип выходного файла!");
        }

        /// <inheritdoc />
        public async Task<string> GetStatisticForUsers(ICollection<string> users, int identifier)
        {
            return await GetStatisticForUsers(users, DateTime.MinValue, identifier).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<string> GetStatisticForQuestions(int identifier)
        {
            if (OutputFileType.Equals(StatisticOutputFileType.Image))
            { }

            if (OutputFileType.Equals(StatisticOutputFileType.Text))
            { }

            throw new ArgumentOutOfRangeException("", OutputFileType, @"Неизвестный тип выходного файла!");
        }
    }
}