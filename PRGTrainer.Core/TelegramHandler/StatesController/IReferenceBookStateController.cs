namespace PRGTrainer.Core.TelegramHandler.StatesController
{
    using JetBrains.Annotations;

    /// <summary>
    /// Интерфейс контроллера состояний для справочника.
    /// </summary>
    public interface IReferenceBookStateController : IStateController
    {
        /// <summary>
        /// Проверяет, использует ли текущий пользователь справочные материалы.
        /// </summary>
        /// <param name="identifier">Идентификатор пользователя.</param>
        /// <returns>Значение, показывающее, что пользователь использует справочные материалы.</returns>
        bool IsUserUsingRefBook([NotNull] int identifier);
    }
}