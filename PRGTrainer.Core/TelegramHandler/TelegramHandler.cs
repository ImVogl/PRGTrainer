namespace PRGTrainer.Core.TelegramHandler
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Threading;
    using TasksStorage;
    using Telegram.Bot;

    /// <summary>
    /// Обработчик взаимодействия с telegram.
    /// </summary>
    public class TelegramHandler : ITelegramHandler
    {
        #region Private fields

        /// <summary>
        /// Клиент telegram.
        /// </summary>
        private readonly ITelegramBotClient _telegramBotClient;

        /// <summary>
        /// Хранилище задач.
        /// </summary>
        private readonly ITasksStorage _tasksStorage;

        /// <summary>
        /// Обработчики текстовых сообщений.
        /// </summary>
        private readonly IEnumerable<IMessageProcessing> _messageProcessors;

        #endregion

        /// <summary>
        /// Конструктор.
        /// </summary>
        /// <param name="tasksStorage">Хранилище задач.</param>
        /// <param name="messageProcessors">Обработчики текстовых сообщений.</param>
        public TelegramHandler(ITasksStorage tasksStorage, IEnumerable<IMessageProcessing> messageProcessors)
        {
            var token = ConfigurationManager.AppSettings[@"telegramToken"];
            _telegramBotClient = new TelegramBotClient(token);
            _tasksStorage = tasksStorage;
            _messageProcessors = messageProcessors;
        }
        
        /// <inheritdoc />
        public void InitialiseSession()
        {
            _tasksStorage.FillStorage();
            foreach (var messageProcessor in _messageProcessors)
                _telegramBotClient.OnMessage += messageProcessor.OnMessage;

            _telegramBotClient.Timeout = new TimeSpan(0, 20, 0);
            _telegramBotClient.StartReceiving();

            Thread.Sleep(Timeout.Infinite);
        }

        /// <inheritdoc />
        public void Dispose()
        {
            _telegramBotClient.StopReceiving();
        }

    }
}