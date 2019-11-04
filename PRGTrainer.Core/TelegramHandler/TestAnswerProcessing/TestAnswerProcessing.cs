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
        /// Состояние пользователя.
        /// </summary>
        private readonly Dictionary<int, UserTestState> _subStates;

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
            _subStates = new Dictionary<int, UserTestState>();
            _telegramBotClient = telegramBotClient;
            _tasksProcessing = tasksProcessing;
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

            if (message.Text == _testStateController.FinishCommand)
            {
                _subStates.Remove(message.From.Id);
                _testStateController.ResetState(message.From.Id);
                return;
            }

            ProcessMessage(message.From.Id, message.From.Username, message.Text);
        }

        #region Private methods

        /// <summary>
        /// Обработка команд.
        /// </summary>
        /// <param name="id">Идентификатор пользователя.</param>
        /// <param name="user">Имя пользователя.</param>
        /// <param name="command">Команда.</param>
        private void ProcessMessage(int id, string user, string command)
        {
            if ((!_subStates.ContainsKey(id) || _subStates[id].SubState == StateChoice) && ChoiceCommands.Contains(command))
            {
                SetQuestions(id, user, command);
                _subStates[id].SubState = StateTest;
                _subStates[id].CurrentTaskNum = 0;
                SendQuestion(id);
                return;
            }

            if (AnswerCommands.Contains(command) && _subStates[id].SubState == StateTest)
            {
                _subStates[id].CurrentTaskNum += 1;
                ProcessAnswer(id, user, command);
                SendQuestion(id);
            }

            if (command == _testStateController.NewTestCommand)
                StartTest(id);
        }

        /// <summary>
        /// Начало тестирования.
        /// </summary>
        /// <param name="id">Идентификатор пользователя.</param>
        private void StartTest(int id)
        {
            const string message = @"Выберете вариант тестов.";
            var keyboard = new ReplyKeyboardMarkup(CreateStartTestButtons());
            _telegramBotClient.SendTextMessageAsync(id, message, replyMarkup: keyboard);
        }

        /// <summary>
        /// Задает коллекцию вопросов.
        /// </summary>
        /// <param name="id">Идентификатор пользователя.</param>
        /// <param name="user">Имя пользователя.</param>
        /// <param name="command">Команда с типом вопросов.</param>
        private void SetQuestions(int id, string user, string command)
        {
            switch (command)
            {
                case ConclusiveRoleMember:
                {
                    _subStates[id] = new UserTestState
                    {
                        User = user,
                        TasksInfos = _tasksStorage.GetTasksForConclusiveMembers(QuestionsNumbers).ToList(),
                        Results = new List<bool>(),
                        SubState = StateChoice
                    };

                    break;
                }

                case ConsultativeRoleMember:
                {
                    _subStates[id] = new UserTestState
                    {
                        User = user,
                        TasksInfos = _tasksStorage.GetTasksForConsultativeMembers(QuestionsNumbers).ToList(),
                        Results = new List<bool>(),
                        SubState = StateChoice

                    };

                    break;
                }

                case Observer:
                {
                    _subStates[id] = new UserTestState
                    {
                        User = user,
                        TasksInfos = _tasksStorage.GetTasksForObservers(QuestionsNumbers).ToList(),
                        CurrentTaskNum = 0,
                        Results = new List<bool>(),
                        SubState = StateChoice

                    };

                    break;
                }
            }
        }

        /// <summary>
        /// Отправляет вопрос пользователю.
        /// </summary>
        /// <param name="id">Идентификатор пользователя.</param>
        private void SendQuestion(int id)
        {
            var questions = AnswerCommands.Zip(_subStates[id].TasksInfos[_subStates[id].CurrentTaskNum].Options,
                (intro, question) => intro + ": " + question + Environment.NewLine);

            var message = string.Format(@"Вопрос {0}/{1}: {2}{3}{4}",
                _subStates[id].CurrentTaskNum + 1,
                _subStates[id].TasksInfos.Count + 1,
                _subStates[id].TasksInfos[_subStates[id].CurrentTaskNum].Question,
                Environment.NewLine,
                string.Join(Environment.NewLine, questions));

            var keyboard = new ReplyKeyboardMarkup(CreateTestButtons());
            _telegramBotClient.SendTextMessageAsync(id, message, replyMarkup: keyboard);
        }

        /// <summary>
        /// Обработка ответа пользователя.
        /// </summary>
        /// <param name="id">Идентификатор пользователя.</param>
        /// <param name="user">Имя пользователя.</param>
        /// <param name="answer">Ответ пользователя.</param>
        private void ProcessAnswer(int id, string user, string answer)
        {
            
            var isAnswerCorrect = _tasksProcessing.IsAnswerCorrect(_subStates[id].TasksInfos[_subStates[id].CurrentTaskNum], AnswerCommands.IndexOf(answer));
            _subStates[id].Results.Add(isAnswerCorrect);
            var message = isAnswerCorrect
                ? "Верно!\n\r"
                : string.Format(@"Неверно!{0}Объяснение: {0}{1}{0}", Environment.NewLine,
                    _subStates[id].TasksInfos[_subStates[id].CurrentTaskNum].Explanation);

            _telegramBotClient.SendTextMessageAsync(id, message);

            if (_subStates[id].CurrentTaskNum == _subStates[id].TasksInfos.Count)
            {
                _testStateController.ResetState(id);
                SaveStatistic(id, user);
                SendResult(id);
            }
        }

        /// <summary>
        /// Сохранение результатов пользователя.
        /// </summary>
        /// <param name="id">Идентификатор пользователя.</param>
        /// <param name="user">Пользователь.</param>
        private void SaveStatistic(int id, string user)
        {
            var result = new List<string>();
            for (var i = 0; i < _subStates[id].TasksInfos.Count; i++)
                if (_subStates[id].Results[i])
                    result.Add(_subStates[id].TasksInfos[i].Question);

            _statisticsCollector.SaveResult(result);
            var successRate = 100 * _subStates[id].Results.Count(c => c) / _subStates[id].Results.Count;
            _statisticsCollector.SaveUserResult(user, successRate);
        }

        /// <summary>
        /// Отправляет результаты прохождения теста пользователю.
        /// </summary>
        /// <param name="id">Идентификатор пользователя.</param>
        private void SendResult(int id)
        {
            var message = string.Format(@"Ваш результат: {0:N2}%", 100 * _subStates[id].Results.Count(c => c) / _subStates[id].Results.Count);
            _telegramBotClient.SendTextMessageAsync(id, message);
        }

        /// <summary>
        /// Создание коллекции кнопок с вариантами ответа.
        /// </summary>
        /// <returns>Коллекция кнопок.</returns>
        private KeyboardButton[][] CreateTestButtons()
        {
            return new[]
            {
                new[] { new KeyboardButton(AnswerCommands[0]) },
                new[] { new KeyboardButton(AnswerCommands[1]) },
                new[] { new KeyboardButton(AnswerCommands[2]) },
                new[] { new KeyboardButton(_testStateController.FinishCommand) }
            };
        }

        /// <summary>
        /// Создание кнопок для страницы с выбором настроек теста.
        /// </summary>
        /// <returns></returns>
        private KeyboardButton[][] CreateStartTestButtons()
        {
            return new[]
            {
                new[] { new KeyboardButton(ConclusiveRoleMember) },
                new[] { new KeyboardButton(ConsultativeRoleMember) },
                new[] { new KeyboardButton(Observer) },
                new[] { new KeyboardButton(_testStateController.FinishCommand) }
            };
        }

        #endregion

    }
}