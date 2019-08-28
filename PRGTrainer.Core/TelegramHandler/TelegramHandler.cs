namespace PRGTrainer.Core.TelegramHandler
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using System.Threading.Tasks;
    using TasksStorage;
    using Telegram.Bot;
    using Telegram.Bot.Args;
    using Telegram.Bot.Types.Enums;
    using Telegram.Bot.Types.ReplyMarkups;

    /// <summary>
    /// Обработчик взаимодействия с telegram.
    /// </summary>
    public class TelegramHandler : ITelegramHandler
    {
        #region Private fields

        /// <summary>
        /// Команда начало работы бота.
        /// </summary>
        private const string Start = @"/start";

        /// <summary>
        /// Клиент telegram.
        /// </summary>
        private readonly ITelegramBotClient _telegramBotClient;

        /// <summary>
        /// Контролер ответов пользователь.
        /// </summary>
        private readonly Dictionary<int, List<Model.Task>> _tasks;

        /// <summary>
        /// Номер текущей задачи.
        /// </summary>
        private readonly Dictionary<int, int> _currentTask;

        /// <summary>
        /// Номер текущей задачи.
        /// </summary>
        private readonly Dictionary<int, string> _currentCorrectAnswer;

        /// <summary>
        /// Число верных ответов.
        /// </summary>
        private readonly Dictionary<int, int> _correctAnswersCount;

        /// <summary>
        /// Хранилище задач.
        /// </summary>
        private readonly ITasksStorage _tasksStorage;

        /// <summary>
        /// Генератор случайных чисел.
        /// </summary>
        private readonly Random _random = new Random();

        #endregion

        /// <summary>
        /// Конструктор.
        /// </summary>
        /// <param name="tasksStorage">Хранилище задач.</param>
        public TelegramHandler(ITasksStorage tasksStorage)
        {
            var token = ConfigurationManager.AppSettings[@"telegramToken"];
            _telegramBotClient = new TelegramBotClient(token);
            _tasks = new Dictionary<int, List<Model.Task>>();
            _currentTask = new Dictionary<int, int>();
            _correctAnswersCount = new Dictionary<int, int>();
            _currentCorrectAnswer = new Dictionary<int, string>();
            _tasksStorage = tasksStorage;
        }
        
        /// <inheritdoc />
        public void InitialiseSession()
        {
            _telegramBotClient.OnMessage += OnMessageEvent;
            _telegramBotClient.OnCallbackQuery += OnCallbackQuery;

            _telegramBotClient.StartReceiving();
        }

        /// <inheritdoc />
        public void Dispose()
        {
            _telegramBotClient.StopReceiving();
        }

        #region Private methods

        /// <summary>
        /// Событие, происходящее при написании сообщения.
        /// </summary>
        /// <param name="sender">Отправитель.</param>
        /// <param name="eventArgs">Аргументы события.</param>
        private async void OnMessageEvent(object sender, MessageEventArgs eventArgs)
        {
            var message = eventArgs.Message;
            if (message == null || message.Type != MessageType.Text)
                return;

            switch (message.Text)
            {
                case Start:
                {
                    await StartAction(message.From.Id).ConfigureAwait(false);
                    break;
                }

                default:
                {
                    await _telegramBotClient.SendTextMessageAsync(message.From.Id, "/start").ConfigureAwait(false);
                    break;
                }
            }
        }

        /// <summary>
        /// Обработка события при нажатии на кнопку.
        /// </summary>
        /// <param name="sender">Отправитель.</param>
        /// <param name="eventArgs">Аргументы нажатия на кнопки.</param>
        /// <returns>Коллекция кнопок.</returns>
        private async void OnCallbackQuery(object sender, CallbackQueryEventArgs eventArgs)
        {
            var id = eventArgs.CallbackQuery.From.Id;
            if (_currentCorrectAnswer[id] == eventArgs.CallbackQuery.Data)
            {
                try
                {
                    await _telegramBotClient.AnswerCallbackQueryAsync(eventArgs.CallbackQuery.Id, @"Верно!")
                        .ConfigureAwait(false);

                    _correctAnswersCount[id]++;
                }
                catch
                {
                    //
                }
            }

            if (_currentTask[id] < _tasks[id].Count)
            {
                _currentTask[id]++;
                await SendQuestion(id).ConfigureAwait(false);
            }
            else
            {
                await SendResult(id).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Действие, совершаемое при старте теста.
        /// </summary>
        /// <param name="id">Идентификатор отправителя.</param>
        /// <returns>Экземпляр задачи.</returns>
        private async Task StartAction(int id)
        {
            // TODO: Добавить пользователю выбор числа вопросов.
            _tasks[id] = _tasksStorage.GetTasks(int.MaxValue).ToList();
            _currentTask[id] = 0;
            _correctAnswersCount[id] = 0;

            await SendQuestion(id).ConfigureAwait(false);
        }

        /// <summary>
        /// Отправка пользователю вопроса.
        /// </summary>
        /// <param name="id">Идентификатор пользователя.</param>
        /// <returns></returns>
        private async Task SendQuestion(int id)
        {
            var currentTaskNum = _currentTask[id];
            var keyboard = new ReplyKeyboardMarkup(
                CreateButtons(
                    _tasks[id][currentTaskNum].CorrectOption,
                    _tasks[id][currentTaskNum].FirstWrongOption,
                    _tasks[id][currentTaskNum].SecondWrongOption));

            _currentCorrectAnswer[id] = _tasks[id][currentTaskNum].CorrectOption;
            await _telegramBotClient
                .SendTextMessageAsync(id, _tasks[id][currentTaskNum].Question, replyMarkup: keyboard)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Создание коллекции вертикальнорасположенных кнопок.
        /// </summary>
        /// <param name="correct">Корректный вариант ответа.</param>
        /// <param name="firstIncorrect">Первый неправильный ответ.</param>
        /// <param name="secondIncorrect">Второй неправильный ответ.</param>
        /// <returns>Коллекция кнопок.</returns>
        private KeyboardButton[][] CreateButtons(string correct, string firstIncorrect, string secondIncorrect)
        {
            var correctAnswerPos = _random.Next(1, 4);
            switch (correctAnswerPos)
            {
                case 1:
                    return new[]
                    {
                        new[] { new KeyboardButton(correct) },
                        new[] { new KeyboardButton(firstIncorrect) },
                        new[] { new KeyboardButton(secondIncorrect) }
                    };
                case 2:
                    return new[]
                    {
                        new[] { new KeyboardButton(firstIncorrect) },
                        new[] { new KeyboardButton(correct) },
                        new[] { new KeyboardButton(secondIncorrect) }
                    };
                    
                case 3:
                    return new[]
                    {
                        new[] { new KeyboardButton(secondIncorrect) },
                        new[] { new KeyboardButton(firstIncorrect) },
                        new[] { new KeyboardButton(correct) }
                    };
                default:
                    throw new ArgumentOutOfRangeException(@"Номер варианта ответа неверный.");
            }
        }

        /// <summary>
        /// Получение результата теста.
        /// </summary>
        /// <param name="id">Идентификатор теста.</param>
        /// <returns>Экземпляр задачи.</returns>
        private async Task SendResult(int id)
        {
            var result = string.Format(@"Ваш результат: {0:N2}%", 100*_correctAnswersCount[id]/_tasks.Count);
            await _telegramBotClient.SendTextMessageAsync(id, result).ConfigureAwait(false);
        }

        #endregion
    }
}