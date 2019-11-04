namespace PRGTrainer.Core.Model
{
    using System;

    /// <summary>
    /// Плоский клас со сведениями о состоянии пользователя.
    /// </summary>
    public class UserState
    {
        /// <summary>
        /// Получает или задает идентификатор пользователя.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Имя пользователя.
        /// </summary>
        public string User { get; set; }

        /// <summary>
        /// Получает или задает состояние пользователя.
        /// </summary>
        public UserStates State { get; set; }

        /// <summary>
        /// Получает или задает время последнего обновления состояния.
        /// </summary>
        public DateTime LastUpdateTime { get; set; }
    }
}