namespace PRGTrainer.Core.AdminHandler
{
    using System;
    using System.Collections.Generic;
    using System.Data.SqlClient;
    using System.Linq;
    using System.Threading.Tasks;
    using JetBrains.Annotations;
    using log4net;
    using Model.Result;
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
        /// Таблица с результатами пользователя.
        /// </summary>
        private const string UserResultsTable = @"UserResults";

        /// <summary>
        /// Таблица со счетчиком неверных ответов на вопросы.
        /// </summary>
        private const string QuestionResultsTable = @"QuestionResults";

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
        public async Task<bool> TryAddNewAdmin(int identifier, string token)
        {
            var query = $"EXECUTE dbo.AddAdmin @Token = {token} @Identifier = {identifier}";
            try
            {
                await _connection.OpenAsync().ConfigureAwait(false);
                using (var command = new SqlCommand(query, _connection))
                {
                    await command.ExecuteNonQueryAsync().ConfigureAwait(false);
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                if (Logger.IsErrorEnabled)
                    Logger.Error(@"Не удалось добавить нового администратора!", exception);
            }
            finally
            {
                _connection.Close();
            }

            return await IsUserAdmin(identifier).ConfigureAwait(false);
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
            if (!await IsUserAdmin(identifier))
                return string.Empty;

            var results = new List<UserResult>();
            var query = $"SELECT username, result, finishtime FROM {UserResultsTable} WHERE username IS NOT NULL";
            if (users != null)
                query = users.Aggregate(query, (current, user) => current + $" OR username = '{user}'");

            query += @";";
            try
            {
                await _connection.OpenAsync().ConfigureAwait(false);
                using (var command = new SqlCommand(query, _connection))
                {
                    var reader = await command.ExecuteReaderAsync().ConfigureAwait(false);
                    while (await reader.ReadAsync().ConfigureAwait(false))
                    {
                        var user = reader.GetString(0);
                        var result = reader.GetInt32(1);
                        var date = reader.GetDateTime(2);
                        if (results.Any(item => item.User == user))
                            results[results.FindIndex(item => item.User == user)].Result[date] = result;
                        else
                            results.Add(new UserResult { User = user, Result = new Dictionary<DateTime, int> { { date, result } } });
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                if (Logger.IsErrorEnabled)
                    Logger.Error(@"Не удалось получить результаты пользователей!", exception);

                return string.Empty;
            }
            finally
            {
                _connection.Close();
            }

            if (OutputFileType.Equals(StatisticOutputFileType.Image))
                return _resultFileGenerator.GenerateAsImage(results);

            if (OutputFileType.Equals(StatisticOutputFileType.Text))
                return _resultFileGenerator.GenerateAsText(results);
            
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
            if (!await IsUserAdmin(identifier))
                return string.Empty;

            var questionResult = new List<QuestionResult>();
            var query = $"SELECT question, wrongcount FROM dbo.{QuestionResultsTable};";
            try
            {
                await _connection.OpenAsync().ConfigureAwait(false);
                using (var command = new SqlCommand(query, _connection))
                {
                    var reader = await command.ExecuteReaderAsync().ConfigureAwait(false);
                    while (await reader.ReadAsync().ConfigureAwait(false))
                        questionResult.Add(new QuestionResult { Question = reader.GetString(0), Quota = reader.GetInt32(1)});
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                if (Logger.IsErrorEnabled)
                    Logger.Error(@"Не удалось получить результаты пользователей!", exception);

                return string.Empty;
            }
            finally
            {
                _connection.Close();
            }

            var sum = questionResult.Aggregate((double)0, (current, item) => current + item.Quota);
            questionResult.ForEach(item => item.Quota = item.Quota/sum);
            if (OutputFileType.Equals(StatisticOutputFileType.Image))
                return _resultFileGenerator.GenerateAsImage(questionResult);

            if (OutputFileType.Equals(StatisticOutputFileType.Text))
                return _resultFileGenerator.GenerateAsText(questionResult);

            throw new ArgumentOutOfRangeException("", OutputFileType, @"Неизвестный тип выходного файла!");
        }
    }
}