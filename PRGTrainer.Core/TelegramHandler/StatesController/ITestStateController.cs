namespace PRGTrainer.Core.TelegramHandler.StatesController
{
    using JetBrains.Annotations;

    /// <summary>
    /// Интерфейс контроллера состояний для тестов.
    /// </summary>
    public interface ITestStateController : IStateController
    {
        /// <summary>
        /// Получает текст сообщения, обозначающее начало нового теста.
        /// </summary>
        string NewTestCommand { get; }

        /// <summary>
        /// Проверяет, проходит ли текущий пользователь тестирование.
        /// </summary>
        /// <param name="identifier">Идентификатор пользователя.</param>
        /// <returns>Значение, показывающее, что пользователь проходит тестирование.</returns>
        bool IsUserTakingTest([NotNull] int identifier);
    }
}