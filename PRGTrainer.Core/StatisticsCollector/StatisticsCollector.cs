namespace PRGTrainer.Core.StatisticsCollector
{
    using System.Collections.Generic;

    /// <summary>
    /// Сборщик статистики.
    /// </summary>
    public class StatisticsCollector : IStatisticsCollector
    {
        /// <inheritdoc />
        public void SaveResult(IEnumerable<string> questions)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc />
        public void SaveUserResult(string user, int successRate)
        {
            throw new System.NotImplementedException();
        }
    }
}