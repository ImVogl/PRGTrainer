namespace PRGTrainer.Core.TelegramHandler.MessageProcessing
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Model.ReferenceBook;
    using ReferenceBookStorage;
    using StatesController;
    using Telegram.Bot;
    using Telegram.Bot.Args;
    using Telegram.Bot.Types.Enums;
    using Telegram.Bot.Types.ReplyMarkups;

    /// <summary>
    /// Обработчик сообщений при работе с книгой.
    /// </summary>
    public class ReferenceBookProcessing : IMessageProcessing
    {
        #region Private fields

        /// <summary>
        /// Кнопка возврата в предыдущий раздел.
        /// </summary>
        private const string BackButton = @"Назад";
        
        /// <summary>
        /// Клиент телеграмма.
        /// </summary>
        private readonly ITelegramBotClient _telegramBotClient;

        /// <summary>
        /// Контроллер состояния пользователя.
        /// </summary>
        private readonly IReferenceBookStateController _testStateController;

        /// <summary>
        /// Хранилище справочника.
        /// </summary>
        private readonly IReferenceBookStorage _referenceBookStorage;
        
        /// <summary>
        /// Текущий уровень справочника.
        /// </summary>
        private readonly Dictionary<int, ReferenceBookPart> _refDeepLevels;

        #endregion

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="ReferenceBookProcessing"/>
        /// </summary>
        /// <param name="telegramBotClient">Клиент телеграмма.</param>
        /// <param name="testStateController">Контроллер состояния пользователя.</param>
        /// <param name="referenceBookStorage">Хранилище справочника.</param>
        public ReferenceBookProcessing(ITelegramBotClient telegramBotClient,
            IReferenceBookStateController testStateController, IReferenceBookStorage referenceBookStorage)
        {
            _refDeepLevels = new Dictionary<int, ReferenceBookPart>();
            _telegramBotClient = telegramBotClient;
            _testStateController = testStateController;
            _referenceBookStorage = referenceBookStorage;
        }

        /// <inheritdoc />
        public async void OnMessage(object sender, MessageEventArgs eventArgs)
        {
            var message = eventArgs.Message;
            if (message == null || message.Type != MessageType.Text)
                return;

            if (!_testStateController.IsUserUsingRefBook(message.From.Id))
                return;

            if (message.Text == _testStateController.FinishCommand)
            {
                _refDeepLevels.Remove(message.From.Id);
                await _testStateController.ResetState(message.From.Id).ConfigureAwait(false);
                return;
            }

            await ProcessMessage(message.From.Id, message.Text).ConfigureAwait(false);
        }

        #region Private region

        /// <summary>
        /// Обработка команд.
        /// </summary>
        /// <param name="id">Идентификатор пользователя.</param>
        /// <param name="command">Команда.</param>
        private async Task ProcessMessage(int id, string command)
        {
            if (!_refDeepLevels.ContainsKey(id))
                _refDeepLevels[id] = _referenceBookStorage.RootReferenceBookParts;

            if (command == BackButton && _refDeepLevels[id].Identifier != 0)
                _refDeepLevels[id] = _referenceBookStorage.GetPartById(_refDeepLevels[id].ParentIdentifier);

            if (_refDeepLevels[id].SubParts != null && _refDeepLevels[id].SubParts.Any(part => part.Name == command))
                _refDeepLevels[id] = _refDeepLevels[id].SubParts.First(part => part.Name == command);

            await SendParts(id).ConfigureAwait(false);
        }

        /// <summary>
        /// Отправляет раздел справочника.
        /// </summary>
        /// <param name="id">Идентификатор пользователя.</param>
        private async Task SendParts(int id)
        {
            var message = _refDeepLevels[id].Content ?? _refDeepLevels[id].Name;
            var keyboard = CreateButtons(_refDeepLevels[id]);
            await _telegramBotClient.SendTextMessageAsync(id, message, replyMarkup: keyboard).ConfigureAwait(false);
        }

        /// <summary>
        /// Создает кнопки.
        /// </summary>
        /// <param name="part">Раздел справочника.</param>
        /// <returns>Набор кнопок.</returns>
        private ReplyKeyboardMarkup CreateButtons(ReferenceBookPart part)
        {
            var referenceButtons = new List<List<KeyboardButton>>();
            if (part.SubParts != null)
                referenceButtons.AddRange(part.SubParts.Select(item => new List<KeyboardButton> {new KeyboardButton(item.Name)}));

            if (part.Identifier != 0)
                referenceButtons.Add(new List<KeyboardButton> { new KeyboardButton(BackButton) });

            referenceButtons.Add(new List<KeyboardButton> { new KeyboardButton(_testStateController.FinishCommand)});
            return new ReplyKeyboardMarkup(referenceButtons);
             
        }

        #endregion
    }
}