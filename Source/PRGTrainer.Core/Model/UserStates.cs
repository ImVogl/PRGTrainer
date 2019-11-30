namespace PRGTrainer.Core.Model
{
    /// <summary>
    /// Перечисление состояний пользователя.
    /// </summary>
    public enum UserStates
    {
        /// <summary>
        /// Пользователь проходит тестирование.
        /// </summary>
        TakingTest,

        /// <summary>
        /// Пользователь использует справочные материалы.
        /// </summary>
        UsingRefBook,

        /// <summary>
        /// Пользователь занимается администрированием.
        /// </summary>
        UsingAdministrative
    }
}