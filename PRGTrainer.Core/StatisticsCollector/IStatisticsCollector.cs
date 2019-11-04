namespace PRGTrainer.Core.StatisticsCollector
{
    using System.Collections.Generic;
    using JetBrains.Annotations;

    /// <summary>
    /// Интерфейс сборщика статистики.
    /// </summary>
    public interface IStatisticsCollector
    {
        /// <summary>
        /// Сохраняет вопросы, на которые пользователь ответил успешно.
        /// </summary>
        /// <param name="questions"></param>
        void SaveResult([NotNull] IEnumerable<string> questions);

        /// <summary>
        /// Сохраняет результат пользователя. 
        /// </summary>
        /// <param name="user">Имя пользователя.</param>
        /// <param name="successRate">Доля успешных ответов.</param>
        void SaveUserResult([NotNull] string user, [NotNull] int successRate);
    }
}