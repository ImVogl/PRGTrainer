namespace PRGTrainer.Core.TelegramHandler.StatesController
{
    /// <summary>
    /// Интерфейс контроллера состояний.
    /// </summary>
    public interface IStateController
    {
        /// <summary>
        /// Команда выхода из блока.
        /// </summary>
        string FinishCommand { get; }

        /// <summary>
        /// Сброс состояния.
        /// </summary>
        /// <param name="userId">Идентификатор пользователя.</param>
        void ResetState(int userId);
    }
}