﻿namespace PRGTrainer.Core.TelegramHandler.StatesController
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Threading.Tasks;
    using MessageProcessing;
    using Model;
    using Telegram.Bot;
    using Telegram.Bot.Args;
    using Telegram.Bot.Types.Enums;
    using Telegram.Bot.Types.ReplyMarkups;

    /// <summary>
    /// Контроллер состояний пользователей.
    /// </summary>
    public class StatesController : ITestStateController, IReferenceBookStateController, IAdminController, IMessageProcessing
    {
        #region Private fields

        /// <summary>
        /// Текст сообщения команды тестирования.
        /// </summary>
        private const string NewTestMessage = "/test";

        /// <summary>
        /// Текст сообщения команды доступа к справочным материалам.
        /// </summary>
        private const string OpenRefBook = "/refbook";

        /// <summary>
        /// Текст сообщения команды администрирования.
        /// </summary>
        private const string Administrative = @"/admin";

        /// <summary>
        /// Коллекция состояний пользователей.
        /// </summary>
        private readonly List<UserState> _states;

        /// <summary>
        /// Клиент Telegram бота.
        /// </summary>
        private readonly ITelegramBotClient _telegramBotClient;

        /// <summary>
        /// Сообщение отправляемое при старте бота.
        /// </summary>
        private readonly string _startMessage;

        /// <summary>
        /// Максимальное время бездействия для пользователя.
        /// </summary>
        private readonly TimeSpan _maxInactivityTime;

        #endregion

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="StatesController"/>
        /// </summary>
        /// <param name="telegramBotClient">Клиент Telegram бота.</param>
        public StatesController(ITelegramBotClient telegramBotClient)
        {
            NewTestCommand = NewTestMessage;
            FinishCommand = @"Закончить";
            _telegramBotClient = telegramBotClient;
            _states = new List<UserState>();
            _maxInactivityTime = new TimeSpan(4, 0, 0);
            _startMessage =
                string.Format(
                    @"Для начала работы с ботом выберете один из вариантов:{0}{1} - начать тестирование{0}{2} - посмотреть справочные материалы",
                    Environment.NewLine, NewTestMessage, OpenRefBook);
        }

        /// <inheritdoc />
        public string NewTestCommand { get; }

        /// <inheritdoc />
        public string FinishCommand { get; }

        #region Public methods

        /// <inheritdoc />
        public bool IsUserTakingTest(int identifier)
        {
            return _states.Any(state => state.Id == identifier &&  state.State == UserStates.TakingTest);
        }

        /// <inheritdoc />
        public bool IsUserUsingRefBook(int identifier)
        {
            return _states.Any(state => state.Id == identifier && state.State == UserStates.UsingRefBook);
        }

        /// <inheritdoc />
        public bool IsUserUsingAdministrative(int identifier)
        {
            return _states.Any(state => state.Id == identifier && state.State == UserStates.UsingAdministrative);
        }

        /// <inheritdoc />
        public async void OnMessage(object sender, MessageEventArgs eventArgs)
        {
            var message = eventArgs.Message;
            if (message == null || message.Type != MessageType.Text)
                return;

            if (_states.All(state => state.Id != message.From.Id))
                await NewUserProcess(message.From.Id, message.From.Username, message.Text).ConfigureAwait(false);
            else
                CheckTimeOut(message.From.Id);
        }

        /// <inheritdoc />
        public async Task ResetState(int userId)
        {
            var currentUserState = _states.Single(state => state.Id == userId);
            _states.Remove(currentUserState);
            await _telegramBotClient.SendTextMessageAsync(userId, _startMessage, replyMarkup: new ForceReplyMarkup()).ConfigureAwait(false);
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Проверка состояния 
        /// </summary>
        /// <param name="id">Идентификатор пользователя.</param>
        private void CheckTimeOut(int id)
        {
            if (_states.All(state => state.Id != id))
                throw new ArgumentOutOfRangeException(nameof(id), @"Не существует пользователя с заданным идентификатором в коллекции состояний.");

            if (_states.Count(state => state.Id == id) != 1)
                throw new DuplicateNameException(@"В коллекции состояний существует два пользователя с одинаковыми идентификаторами.");

            var currentUserState = _states.Single(state => state.Id == id);
            if (currentUserState.LastUpdateTime - DateTime.Now > _maxInactivityTime)
                _states.Remove(currentUserState);
            else
                _states[_states.IndexOf(currentUserState)].LastUpdateTime = DateTime.Now;
        }

        /// <summary>
        /// Обработка сообщения от нового пользователя.
        /// </summary>
        /// <param name="id">Идентификатор пользователя.</param>
        /// <param name="user">Имя пользователя.</param>
        /// <param name="text">Текст сообщения пользователя.</param>
        private async Task NewUserProcess(int id, string user, string text)
        {
            switch (text)
            {
                case NewTestMessage:
                {
                    _states.Add(new UserState { Id = id, User = user, State = UserStates.TakingTest, LastUpdateTime = DateTime.Now });
                    break;
                }

                case OpenRefBook:
                {
                    _states.Add(new UserState { Id = id, User = user, State = UserStates.UsingRefBook, LastUpdateTime = DateTime.Now });
                    break;
                }

                case Administrative:
                {
                    _states.Add(new UserState { Id = id, User = user, State = UserStates.UsingAdministrative, LastUpdateTime = DateTime.Now });
                    break;
                }

                default:
                {
                   await _telegramBotClient.SendTextMessageAsync(id, _startMessage).ConfigureAwait(false);
                   break;
                }
            }
        }

        #endregion
    }
}