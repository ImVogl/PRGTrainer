namespace PRGTrainer.Core.TelegramHandler.TestAnswerProcessing
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Model;
    using StatesController;
    using StatisticsCollector;
    using TasksProcessing;
    using TasksStorage;
    using Telegram.Bot;
    using Telegram.Bot.Args;
    using Telegram.Bot.Types.Enums;
    using Telegram.Bot.Types.ReplyMarkups;

    /// <summary>
    /// Обработчик сообщений 
    /// </summary>
    public class TestAnswerProcessing : IMessageProcessing
    {
        #region Private constants

        /// <summary>
        /// Состояние "выбор".
        /// </summary>
        private const string StateChoice = @"choice";

        /// <summary>
        /// Состояние "тестирование".
        /// </summary>
        private const string StateTest = @"test";

        /// <summary>
        /// Текст команды тестирования для членов комиссии с правом решающего голоса.
        /// </summary>
        private const string ConclusiveRoleMember = "ПРГ";

        /// <summary>
        /// Текст команды тестирования для членов комиссии с правом совещательного голоса.
        /// </summary>
        private const string ConsultativeRoleMember = @"ПСГ";

        /// <summary>
        /// Текст команды тестирования для наблюдателя.
        /// </summary>
        private const string Observer = @"Наблюдатель";

        /// <summary>
        /// Число вопросов, на которые предстоит ответить пользователю.
        /// </summary>
        private const int QuestionsNumbers = 25;

        #endregion

        #region Private fields

        /// <summary>
        /// Коллекция с командами выбора.
        /// </summary>
        private static readonly List<string> ChoiceCommands = new List<string> { ConclusiveRoleMember, ConsultativeRoleMember, Observer };

        /// <summary>
        /// Коллекция с командами ответов на вопросы.
        /// </summary>
        private static readonly List<string> AnswerCommands = new List<string> { @"Первый вариант", @"Второй вариант", @"Третий вариант" };

        /// <summary>
        /// Контроллер состояния пользователя.
        /// </summary>
        private readonly ITestStateController _testStateController;

        /// <summary>
        /// Хранилище задач.
        /// </summary>
        private readonly ITasksStorage _tasksStorage;

        /// <summary>
        /// Коллекция задач для пользователя с заданным именем.
        /// </summary>
        private readonly Dictionary<string, List<TaskInfo>> _tasks;

        /// <summary>
        /// Номер текущей задачи для пользователя с заданным именем.
        /// </summary>
        private readonly Dictionary<string, int> _currentTaskNum;

        /// <summary>
        /// Словарь с результатами теста для пользователя.
        /// </summary>
        private readonly Dictionary<string, List<bool>> _testResults;

        /// <summary>
        /// Обработчик задач.
        /// </summary>
        private readonly ITasksProcessing _tasksProcessing;

        /// <summary>
        /// Клиент telegram.
        /// </summary>
        private readonly ITelegramBotClient _telegramBotClient;

        /// <summary>
        /// Сборщик статистики.
        /// </summary>
        private readonly IStatisticsCollector _statisticsCollector;

        /// <summary>
        /// Состояние пользователя внутри теста.
        /// </summary>
        private string _subState;

        #endregion

        /// <summary>
        /// Инициализирует экземпляр <see cref="TestAnswerProcessing"/>
        /// </summary>
        /// <param name="telegramBotClient">Клиент telegram.</param>
        /// <param name="testStateController">Контроллер состояния пользователя.</param>
        /// <param name="tasksStorage">Хранилище задач.</param>
        /// <param name="tasksProcessing">Обработчик задач.</param>
        /// <param name="statisticsCollector">Сборщик статистики.</param>
        public TestAnswerProcessing(ITelegramBotClient telegramBotClient, ITestStateController testStateController,
            ITasksStorage tasksStorage, ITasksProcessing tasksProcessing, IStatisticsCollector statisticsCollector)
        {
            _tasks = new Dictionary<string, List<TaskInfo>>();
            _currentTaskNum = new Dictionary<string, int>();
            _testResults = new Dictionary<string, List<bool>>();
            _telegramBotClient = telegramBotClient;
            _tasksProcessing = tasksProcessing;
            _subState = StateChoice;
            _testStateController = testStateController;
            _tasksStorage = tasksStorage;
            _statisticsCollector = statisticsCollector;
        }

        /// <inheritdoc />
        public void OnMessage(object sender, MessageEventArgs eventArgs)
        {
            var message = eventArgs.Message;
            if (message == null || message.Type != MessageType.Text)
                return;

            if (!_testStateController.IsUserTakingTest(message.From.Id))
                return;

            if (message.Text == _testStateController.NewTestCommand)
                return;

            if (message.Text == _testStateController.FinishCommand)
            {
                _testStateController.ResetState(message.From.Id);
                return;
            }

            ProcessMessage(message.From.Username, message.Text);
        }

        #region Private methods

        /// <summary>
        /// Обработка команд.
        /// </summary>
        /// <param name="user">Имя пользователя.</param>
        /// <param name="command">Команда.</param>
        private void ProcessMessage(string user, string command)
        {
            switch (_subState)
            {
                case StateChoice:
                {
                    if (ChoiceCommands.Contains(command))
                    {
                        _subState = StateTest;
                        _currentTaskNum[user] = 0;
                        _testResults[user] = new List<bool>(QuestionsNumbers);
                        SetQuestions(user, command);
                        SendQuestion(user);
                    }

                    break;
                }

                case StateTest:
                {
                    if (AnswerCommands.Contains(command))
                    {
                        _currentTaskNum[user] += 1;
                        ProcessAnswer(user, command);
                        SendQuestion(user);
                    }

                    break;
                }
            }
        }

        /// <summary>
        /// Задает коллекцию вопросов.
        /// </summary>
        /// <param name="user">Имя пользователя.</param>
        /// <param name="command">Команда с типом вопросов.</param>
        private void SetQuestions(string user, string command)
        {
            switch (command)
            {
                case ConclusiveRoleMember:
                {
                    _tasks[user] = _tasksStorage.GetTasksForConclusiveMembers(QuestionsNumbers).ToList();
                    break;
                }

                case ConsultativeRoleMember:
                {
                    _tasks[user] = _tasksStorage.GetTasksForConsultativeMembers(QuestionsNumbers).ToList();
                    break;
                }

                case Observer:
                {
                    _tasks[user] = _tasksStorage.GetTasksForObservers(QuestionsNumbers).ToList();
                    break;
                }
            }
        }

        /// <summary>
        /// Отправляет вопрос пользователю.
        /// </summary>
        /// <param name="user">Имя пользователя.</param>
        private void SendQuestion(string user)
        {
            var questions = AnswerCommands.Zip(_tasks[user][_currentTaskNum[user]].Options,
                (intro, question) => intro + " " + question);

            var message = string.Format(@"Вопрос {0}/{1}:{2}{3}{4}", 
                _currentTaskNum[user] + 1,
                _tasks[user].Count + 1,
                _tasks[user][_currentTaskNum[user]].Question,
                Environment.NewLine,
                string.Join(Environment.NewLine, questions));

            var keyboard = new ReplyKeyboardMarkup(CreateButtons());
            _telegramBotClient.SendTextMessageAsync(user, message, replyMarkup: keyboard);
        }

        /// <summary>
        /// Обработка ответа пользователя.
        /// </summary>
        /// <param name="user">Имя пользователя.</param>
        /// <param name="answer">Ответ пользователя.</param>
        private void ProcessAnswer(string user, string answer)
        {
            var isAnswerCorrect = _tasksProcessing.IsAnswerCorrect(_tasks[user][_currentTaskNum[user]], AnswerCommands.IndexOf(answer));
            _testResults[user][_currentTaskNum[user]] = isAnswerCorrect;
            var message = isAnswerCorrect
                ? "Верно!\n\r"
                : string.Format(@"Неверно!{0}Объяснение: {0}{1}{0}", Environment.NewLine,
                    _tasks[user][_currentTaskNum[user]].Explanation);

            _telegramBotClient.SendTextMessageAsync(user, message);

            if (_currentTaskNum[user] == _tasks[user].Count)
            {
                _testStateController.ResetState(user);
                SaveStatistic(user);
                SendResult(user);
            }
        }

        /// <summary>
        /// Сохранение результатов пользователя.
        /// </summary>
        /// <param name="user">Пользователь.</param>
        private void SaveStatistic(string user)
        {
            var result = new List<string>();
            for (var i = 0; i < _tasks[user].Count; i++)
                if (_testResults[user][i])
                    result.Add(_tasks[user][i].Question);

            _statisticsCollector.SaveResult(result);
            var successRate = 100 * _testResults[user].Count(c => c) / _testResults[user].Count;
            _statisticsCollector.SaveUserResult(user, successRate);
        }

        /// <summary>
        /// Отправляет результаты прохождения теста пользователю.
        /// </summary>
        /// <param name="user">Пользователь.</param>
        private void SendResult(string user)
        {
            var message = string.Format(@"Ваш результат: {0:N2}%", 100 * _testResults[user].Count(c => c) / _testResults[user].Count);
            _telegramBotClient.SendTextMessageAsync(user, message);
        }

        /// <summary>
        /// Создание коллекции вертикально расположенных кнопок.
        /// </summary>
        /// <returns>Коллекция кнопок.</returns>
        private KeyboardButton[][] CreateButtons()
        {
            return new[]
            {
                new[] { new KeyboardButton(AnswerCommands[0]) },
                new[] { new KeyboardButton(AnswerCommands[1]) },
                new[] { new KeyboardButton(AnswerCommands[2]) }
            };
        }

        #endregion

    }
}