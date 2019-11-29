namespace PRGTrainer.Core.TelegramHandler.MessageProcessing
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using AdminHandler;
    using Help.ParsingArguments;
    using StatesController;
    using Telegram.Bot;
    using Telegram.Bot.Args;
    using Telegram.Bot.Types.Enums;
    using Telegram.Bot.Types.InputFiles;

    /// <summary>
    /// Обработчик сообщений при администрировании.
    /// </summary>
    public class AdminProcessing : IMessageProcessing
    {
        #region Commands

        /// <summary>
        /// Ключ с выходным типом.
        /// </summary>
        private const string OutTypeKey = @"/out:";

        /// <summary>
        /// Команда, обозначающее, что выходной тип - изображение.
        /// </summary>
        private const string OutTypeImg = @"img";

        /// <summary>
        /// Ключ с датой, начиная с которой нужно взять статистику.
        /// </summary>
        private const string StartDateKey = @"/date:";

        /// <summary>
        /// Ключ задания пользователей, для которых нужно получит статистику.
        /// </summary>
        private const string UsersKey = @"/users:";

        /// <summary>
        /// Ключ получения статистики пользователей.
        /// </summary>
        private const string UserStatisticKey = @"/userstat:";

        /// <summary>
        /// Ключ получения статистики по вопросам.
        /// </summary>
        private const string QuestionStatisticKey = @"/questionsstat:";

        /// <summary>
        /// Ключ ввода токена доступа.
        /// </summary>
        private const string TokenKey = @"/token:";

        #endregion

        #region Private fields

        /// <summary>
        /// Клиент телеграмма.
        /// </summary>
        private readonly ITelegramBotClient _telegramBotClient;

        /// <summary>
        /// Контроллер состояния пользователя.
        /// </summary>
        private readonly IAdminController _testStateController;

        /// <summary>
        /// Обработчик действий администратора.
        /// </summary>
        private readonly IAdminHandler _adminHandler;

        /// <summary>
        /// Парсер аргументов.
        /// </summary>
        private readonly IArgumentParser _argumentParser;

        /// <summary>
        /// Словарь с типами выходных файлов со статистикой.
        /// </summary>
        private readonly Dictionary<int, StatisticOutputFileType> _outputFileTypes;

        #endregion

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="AdminProcessing"/>
        /// </summary>
        /// <param name="telegramBotClient">Клиент телеграмма.</param>
        /// <param name="adminController">Контроллер состояния пользователя.</param>
        /// <param name="adminHandler">Обработчик действий администратора.</param>
        /// <param name="argumentParser">Парсер аргументов.</param>
        public AdminProcessing(
            ITelegramBotClient telegramBotClient, 
            IAdminController adminController,
            IAdminHandler adminHandler,
            IArgumentParser argumentParser)
        {
            _telegramBotClient = telegramBotClient;
            _testStateController = adminController;
            _adminHandler = adminHandler;
            _argumentParser = argumentParser;
            _outputFileTypes = new Dictionary<int, StatisticOutputFileType>();
        }

        /// <inheritdoc />
        public async void OnMessage(object sender, MessageEventArgs eventArgs)
        {
            var message = eventArgs.Message;
            if (message == null || message.Type != MessageType.Text)
                return;

            if (!_testStateController.IsUserUsingAdministrative(message.From.Id))
                return;

            if (message.Text == _testStateController.FinishCommand)
                await _testStateController.ResetState(message.From.Id).ConfigureAwait(false);
            else
                await ProcessMessage(message.From.Id, message.Text).ConfigureAwait(false);
        }

        #region Private methods

        /// <summary>
        /// Обработка команд.
        /// </summary>
        /// <param name="id">Идентификатор пользователя.</param>
        /// <param name="command">Команда.</param>
        /// <returns>Задача.</returns>
        private async Task ProcessMessage(int id, string command)
        {

            if (!await _adminHandler.IsUserAdmin(id).ConfigureAwait(false))
            {
                var message =
                    string.Format(
                        @"Не удалось найти вас в списке администраторов.{0}Введите токен доступа следующим образом:{1}token",
                        Environment.NewLine, TokenKey);

                await _telegramBotClient.SendTextMessageAsync(id, message).ConfigureAwait(false);
                return;
            }

            if (command.Contains(TokenKey))
            {
                if (!await _adminHandler.TryAddNewAdmin(_argumentParser.Parse(command, TokenKey)).ConfigureAwait(false))
                {
                    const string message = "Не удалось добавить вас в списке администраторов.\nВероятно, вы ввели неверный токен.";
                    await _telegramBotClient.SendTextMessageAsync(id, message).ConfigureAwait(false);
                    return;
                }
            }

            SetOutFileType(id, command);
            if (command.Contains(QuestionStatisticKey))
            {
                await GetQuestionStatistic(id).ConfigureAwait(false);
                return;
            }

            if (command.Contains(UserStatisticKey))
                await GetUserStatistic(id, command).ConfigureAwait(false);

        }

        /// <summary>
        /// Получение статистики по вопросам.
        /// </summary>
        /// <param name="id">Идентификатор пользователя.</param>
        /// <returns>Задача.</returns>
        private async Task GetQuestionStatistic(int id)
        {
            var path = await _adminHandler.GetStatisticForQuestions(id).ConfigureAwait(false);
            await SendDocument(id, path).ConfigureAwait(false);
        }

        /// <summary>
        /// Получение статистики по пользователям.
        /// </summary>
        /// <param name="id">Идентификатор пользователя.</param>
        /// <param name="command">Команда.</param>
        /// <returns>Задача.</returns>
        private async Task GetUserStatistic(int id, string command)
        {
            string path;
            var users = _argumentParser.ParseCollection(command, UsersKey).ToList();
            if (command.Contains(StartDateKey))
                path = await _adminHandler.GetStatisticForUsers(users, GetDateTime(command), id).ConfigureAwait(false);
            else
                path = await _adminHandler.GetStatisticForUsers(users, id).ConfigureAwait(false);

            await SendDocument(id, path).ConfigureAwait(false);
        }

        /// <summary>
        /// Отправляет файл.
        /// </summary>
        /// <param name="id">Идентификатор пользователя.</param>
        /// <param name="path">Путь до файла.</param>
        /// <returns>Задача отправки документа пользователю.</returns>
        private async Task SendDocument(int id, string path)
        {
            using (var stream = new FileStream(path, FileMode.Open))
            {
                var file = new InputOnlineFile(stream, @"Result");
                await _telegramBotClient.SendDocumentAsync(id, file);
            }
        }

        /// <summary>
        /// Получение даты.
        /// </summary>
        /// <param name="command">Команда.</param>
        /// <returns>Дата.</returns>
        private DateTime GetDateTime(string command)
        {
            var date = _argumentParser.Parse(command, StartDateKey);
            return DateTime.TryParse(date, out var result) 
                ? result 
                : DateTime.MinValue;
        }

        /// <summary>
        /// Задание типа выходного файла.
        /// </summary>
        /// <param name="id">Идентификатор.</param>
        /// <param name="command">Команда.</param>
        private void SetOutFileType(int id, string command)
        {
            if (command.Contains(OutTypeKey))
                _outputFileTypes[id] = 
                    _argumentParser.Parse(command, OutTypeKey) == OutTypeImg
                        ? StatisticOutputFileType.Image
                        : StatisticOutputFileType.Text;

            _adminHandler.OutputFileType = _outputFileTypes[id];
        }

        #endregion
    }
}