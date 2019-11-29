namespace PRGTrainer.Core.Model.Result
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Результаты пользователя.
    /// </summary>
    public class UserResult
    {
        /// <summary>
        /// Получает или задает имя пользователя.
        /// </summary>
        public string User { get; set; }

        /// <summary>
        /// Получает или задает результат пользователя.
        /// </summary>
        public Dictionary<DateTime, int> Result { get; set; }
    }
}