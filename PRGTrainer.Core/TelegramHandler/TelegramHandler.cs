namespace PRGTrainer.Core.TelegramHandler
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using System.Threading;
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
        #region Private constants

        /// <summary>
        /// Команда начало работы бота.
        /// </summary>
        private const string Start = @"/start";

        /// <summary>
        /// Команда на перезапуск теста.
        /// </summary>
        private const string Restart = @"/restart";

        /// <summary>
        /// Команда для первого варианта ответа.
        /// </summary>
        private const string FirstOption = @"Первый вариант";

        /// <summary>
        /// Команда для второго варианта ответа.
        /// </summary>
        private const string SecondOption = @"Второй вариант";

        /// <summary>
        /// Команда для третьего варианта ответа.
        /// </summary>
        private const string ThirdOption = @"Третий вариант";

        #endregion

        #region Private fields

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
            _tasksStorage.FillStorage();
            _telegramBotClient.OnMessage += OnMessageEvent;
            _telegramBotClient.StartReceiving();

            Thread.Sleep(Timeout.Infinite);
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

            var text = message.Text;
            if (text == Start)
            {
                await StartAction(message.From.Id).ConfigureAwait(false);
                return;
            }

            if (text == FirstOption || text == SecondOption || text == ThirdOption)
            {
                await AnswerProcessing(message.From.Id, text).ConfigureAwait(false);
                return;
            }

            if (text == Restart)
            {
                var keyboard = new ReplyKeyboardMarkup(new[] { new KeyboardButton(Start) });
                await _telegramBotClient.SendTextMessageAsync(message.From.Id, Start, replyMarkup: keyboard).ConfigureAwait(false);
            }

        }

        /// <summary>
        /// Обработка ответа пользователя.
        /// </summary>
        /// <param name="id">Идентификатор пользователя.</param>
        /// <param name="answerOption">Вариант ответа пользователя.</param>
        /// <returns></returns>
        private async Task AnswerProcessing(int id, string answerOption)
        {
            if (_currentCorrectAnswer[id] == answerOption)
                _correctAnswersCount[id]++;
            
            await SendExplanation(id).ConfigureAwait(false);

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
        /// Отправление объяснения к вопросу.
        /// </summary>
        /// <param name="id">Идентификатор пользователя.</param>
        /// <returns>Экземпляр задачи.</returns>
        private async Task SendExplanation(int id)
        {
            var num = _currentTask[id];
            if (string.IsNullOrWhiteSpace(_tasks[id][num].Explanation))
                return;

            var message = string.Format(@"Объяснение: {0}{1}{0}", Environment.NewLine, _tasks[id][num].Explanation);
            await _telegramBotClient.SendTextMessageAsync(id, message).ConfigureAwait(false);
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
            var keyboard = new ReplyKeyboardMarkup(CreateButtons());

            var taskString = BuildQuestionMessage(_tasks[id][currentTaskNum], out var correctOption);
            _currentCorrectAnswer[id] = correctOption;
            await _telegramBotClient
                .SendTextMessageAsync(id, taskString, replyMarkup: keyboard)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Создание сообщения с вопросом.
        /// </summary>
        /// <param name="task">Задача.</param>
        /// <param name="correctOption">Вариант с правильным ответом.</param>
        /// <returns>Строка с вопросом.</returns>
        private string BuildQuestionMessage(Model.Task task, out string correctOption)
        {
            var correctAnswerPos = _random.Next(1, 4);
            switch (correctAnswerPos)
            {
                case 1:
                {
                    correctOption = FirstOption;
                    return string.Format(@"{0}{1}{1}{2}:{1}{3}{1}{1}{4}:{1}{5}{1}{1}{6}:{1}{7}", task.Question,
                        Environment.NewLine, FirstOption, task.CorrectOption, SecondOption, task.FirstWrongOption,
                        ThirdOption, task.SecondWrongOption);
                }
                case 2:
                {
                    correctOption = SecondOption;
                    return string.Format(@"{0}{1}{1}{2}:{1}{3}{1}{1}{4}:{1}{5}{1}{1}{6}:{1}{7}", task.Question,
                        Environment.NewLine, FirstOption, task.FirstWrongOption, SecondOption, task.CorrectOption,
                        ThirdOption, task.SecondWrongOption);
                    }

                case 3:
                {
                    correctOption = ThirdOption;
                    return string.Format(@"{0}{1}{1}{2}:{1}{3}{1}{1}{4}:{1}{5}{1}{1}{6}:{1}{7}", task.Question,
                            Environment.NewLine, FirstOption, task.FirstWrongOption, SecondOption, task.SecondWrongOption,
                            ThirdOption, task.CorrectOption);
                    }
                default:
                    throw new ArgumentOutOfRangeException(@"Номер варианта ответа неверный.");
            }
        }

        /// <summary>
        /// Создание коллекции вертикально расположенных кнопок.
        /// </summary>
        /// <returns>Коллекция кнопок.</returns>
        private KeyboardButton[][] CreateButtons()
        {
            return new[]
            {
                new[] { new KeyboardButton(FirstOption) },
                new[] { new KeyboardButton(SecondOption) },
                new[] { new KeyboardButton(ThirdOption) }
            };
                
            
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