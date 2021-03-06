﻿namespace PRGTrainer.Core.StatisticsCollector
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using JetBrains.Annotations;

    /// <summary>
    /// Интерфейс сборщика статистики.
    /// </summary>
    public interface IStatisticsCollector
    {
        /// <summary>
        /// Сохраняет вопросы, на которые пользователь ответил неверно.
        /// </summary>
        /// <param name="questions"></param>
        Task SaveResult([NotNull] IEnumerable<string> questions);

        /// <summary>
        /// Сохраняет результат пользователя. 
        /// </summary>
        /// <param name="id">Идентификатор пользователя.</param>
        /// <param name="user">Имя пользователя.</param>
        /// <param name="successRate">Доля успешных ответов.</param>
        Task SaveUserResult([NotNull] int id, [CanBeNull] string user, [NotNull] int successRate);
    }
}