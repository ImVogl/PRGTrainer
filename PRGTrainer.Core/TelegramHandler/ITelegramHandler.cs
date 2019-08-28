namespace PRGTrainer.Core.TelegramHandler
{
    /// <summary>
    /// Интерфейс взаимодействия с сервером telegram.
    /// </summary>
    public interface ITelegramHandler
    {
        /// <summary>
        /// Инициализация сессии.
        /// </summary>
        void InitialiseSession();
    }
}