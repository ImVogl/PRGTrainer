namespace PRGTrainer.Core.TelegramHandler.StatesController
{
    using System.Threading.Tasks;

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
        Task ResetState(int userId);
    }
}