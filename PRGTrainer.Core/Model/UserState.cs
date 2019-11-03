namespace PRGTrainer.Core.Model
{
    using System;

    /// <summary>
    /// Плоский клас со сведениями о состоянии пользователя.
    /// </summary>
    public class UserState
    {
        /// <summary>
        /// Идентификатор пользователя.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Состояние пользователя.
        /// </summary>
        public UserStates State { get; set; }

        /// <summary>
        /// Время последнего обновления состояния.
        /// </summary>
        public DateTime LastUpdateTime { get; set; }
    }
}