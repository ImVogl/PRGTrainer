namespace PRGTrainer.Core.TelegramHandler
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using JetBrains.Annotations;
    using log4net.Config;
    using MessageProcessing;
    using ReferenceBookStorage;
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
        /// Хранилище справочника.
        /// </summary>
        private readonly IReferenceBookStorage _referenceBookStorage;

        /// <summary>
        /// Обработчики текстовых сообщений.
        /// </summary>
        private readonly IEnumerable<IMessageProcessing> _messageProcessors;

        /// <summary>
        /// Токен отмены бесконечной операции.
        /// </summary>
        private readonly CancellationTokenSource _cancellationToken;

        #endregion

        /// <summary>
        /// Конструктор.
        /// </summary>
        /// <param name="tasksStorage">Хранилище задач.</param>
        /// <param name="referenceBookStorage">Хранилище справочника.</param>
        /// <param name="messageProcessors">Обработчики текстовых сообщений.</param>
        /// <param name="telegramBotClient">Клиент telegram.</param>
        public TelegramHandler(ITasksStorage tasksStorage, IReferenceBookStorage referenceBookStorage,
            IEnumerable<IMessageProcessing> messageProcessors, [NotNull] ITelegramBotClient telegramBotClient)
        {
            _telegramBotClient = telegramBotClient;
            _tasksStorage = tasksStorage;
            _referenceBookStorage = referenceBookStorage;
            _messageProcessors = messageProcessors;
            _cancellationToken = new CancellationTokenSource();
        }
        
        /// <inheritdoc />
        public void InitialiseSession()
        {
            XmlConfigurator.Configure();
            _tasksStorage.FillStorage();
            _referenceBookStorage.FillStorage();
            foreach (var messageProcessor in _messageProcessors)
                _telegramBotClient.OnMessage += messageProcessor.OnMessage;

            _telegramBotClient.Timeout = new TimeSpan(0, 20, 0);
            _telegramBotClient.StartReceiving();

            Task.Run(async () => await Task.Delay(Timeout.Infinite, _cancellationToken.Token).ConfigureAwait(false));
        }

        /// <inheritdoc />
        public void Dispose()
        {
            _telegramBotClient.StopReceiving();
            _cancellationToken.Cancel();
        }

    }
}