namespace PRGTrainer.Core.StatisticsCollector
{
    using System;
    using System.Collections.Generic;
    using System.Data.SqlClient;

    /// <summary>
    /// Сборщик статистики.
    /// </summary>
    public class StatisticsCollector : IStatisticsCollector
    {
        #region Private fields
        
        /// <summary>
        /// Таблица с результатами пользователя.
        /// </summary>
        private const string UserResultTable = @"UserResults";

        /// <summary>
        /// Таблица с коллекцией вопросов.
        /// </summary>
        private const string QuestionResultTable = @"QuestionResults";

        /// <summary>
        /// Строка подключения к бд для статистики пользователя.
        /// </summary>
        private readonly string _connectionString;

        #endregion

        /// <summary>
        /// Инициализирует экземпляр <see cref="StatisticsCollector"/>
        /// </summary>
        public StatisticsCollector()
        {
            _connectionString = System.Configuration.ConfigurationManager.ConnectionStrings[@"UserStatistics"].ConnectionString;
        }
        
        /// <inheritdoc />
        public async void SaveResult(IEnumerable<string> questions)
        {
            var query = "DECLARE @val int;\n\r";
            foreach (var question in questions)
                query += string.Format(@"IF EXISTS (SELECT 1 FROM dbo.{0} WHERE question = '{1}')
BEGIN
	SET @val = (SELECT wrongcount FROM dbo.{0} WHERE question = '{1}') + 1;
	UPDATE dbo.{0} SET wrongcount = @val  WHERE question = '{1}';
END
ELSE
BEGIN
	INSERT INTO dbo.{0} (question, wrongcount) VALUES ('{1}', 1);
END{2}{2}", QuestionResultTable, question, Environment.NewLine);

            var connection = new SqlConnection(_connectionString);
            try
            {
                await connection.OpenAsync().ConfigureAwait(false);
                using (var command = new SqlCommand(query, connection))
                {
                    await command.ExecuteNonQueryAsync().ConfigureAwait(false);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            finally
            {
                connection.Close();
            }
        }

        /// <inheritdoc />
        public async void SaveUserResult(int id, string user, int successRate)
        {

            var connection = new SqlConnection(_connectionString);
            try
            {

                await connection.OpenAsync().ConfigureAwait(false);
                var query = string.IsNullOrWhiteSpace(user)
                    ? $"INSERT INTO dbo.{UserResultTable} (identifier, result, finishtime) VALUES ({id}, {successRate}, {GetDataTime()});"
                    : $"INSERT INTO dbo.{UserResultTable} (identifier, username, result, finishtime) VALUES ({id}, '{user}', {successRate}, {GetDataTime()});";

                using (var command = new SqlCommand(query, connection))
                {
                    await command.ExecuteNonQueryAsync().ConfigureAwait(false);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            finally
            {
                connection.Close();
            }
        }

        /// <summary>
        /// Получает время и дату для SQL
        /// </summary>
        /// <returns>Текущее дата и время.</returns>
        private string GetDataTime()
        {
            var dateTime = DateTime.Now;
            return string.Format(
                @"convert(datetime2, '{0}-{1}-{2} {3}:{4}:{5}')", 
                dateTime.Year,
                ConvertLessToTenValues(dateTime.Month),
                ConvertLessToTenValues(dateTime.Day),
                ConvertLessToTenValues(dateTime.Hour),
                ConvertLessToTenValues(dateTime.Minute),
                ConvertLessToTenValues(dateTime.Second));
        }

        /// <summary>
        /// Добавляет значениям меньше, чем 10 ноль
        /// </summary>
        /// <param name="value">Значение.</param>
        /// <returns>Откорректированное значение.</returns>
        private string ConvertLessToTenValues(int value)
        {
            return value < 10 ? $"0{value}" : value.ToString();
        }
    }
}