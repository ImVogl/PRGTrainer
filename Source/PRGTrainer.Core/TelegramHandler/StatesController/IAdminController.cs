namespace PRGTrainer.Core.TelegramHandler.StatesController
{
    using JetBrains.Annotations;

    /// <summary>
    /// Интерфейс контроллера состояний при администрировании.
    /// </summary>
    public interface IAdminController : IStateController
    {
        /// <summary>
        /// Проверяет, занимается ли текущий пользователь администрированием.
        /// </summary>
        /// <param name="identifier">Идентификатор пользователя.</param>
        /// <returns>Результат, показывающий, что пользователь занимается администрированием.</returns>
        bool IsUserUsingAdministrative([NotNull] int identifier);
    }
}