namespace PRGTrainer.Core.TelegramHandler
{
    using Telegram.Bot.Args;

    /// <summary>
    /// Интерфейс обработчика ответов сервера с текстовыми сообщениями.
    /// </summary>
    public interface IMessageProcessing
    {
        /// <summary>
        /// Событие, происходящее при написании сообщения.
        /// </summary>
        /// <param name="sender">Отправитель.</param>
        /// <param name="eventArgs">Аргументы события.</param>
        void OnMessage(object sender, MessageEventArgs eventArgs);
    }
}