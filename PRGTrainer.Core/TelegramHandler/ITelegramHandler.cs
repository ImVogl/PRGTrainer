namespace PRGTrainer.Core.TelegramHandler
{
    using System;
    using TasksStorage;

    /// <summary>
    /// Интерфейс взаимодействия с сервером telegram.
    /// </summary>
    public interface ITelegramHandler : IDisposable
    {
        /// <summary>
        /// Инициализация сессии.
        /// </summary>
        void InitialiseSession();
    }
}