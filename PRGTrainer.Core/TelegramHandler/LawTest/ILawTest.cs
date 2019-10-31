namespace PRGTrainer.Core.TelegramHandler.LawTest
{
    using System.Threading.Tasks;

    /// <summary>
    /// Интерфейс тестирования.
    /// </summary>
    public interface ILawTest
    {
        /// <summary>
        /// Обработка ответа пользователя.
        /// </summary>
        /// <param name="id">Идентификатор пользователя.</param>
        /// <param name="answer">Вариант ответа пользователя.</param>
        /// <returns>Экземпляр задачи.</returns>
        Task AnswerProcessing(int id, string answer);
    }
}