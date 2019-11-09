namespace PRGTrainer.Core.Tests
{
    using System.Collections.Generic;
    using System.IO;
    using Model.Test;
    using Moq;
    using NUnit.Framework;
    using StatisticsCollector;
    using TasksProcessing;
    using TasksStorage;
    using Telegram.Bot;
    using Telegram.Bot.Args;
    using Telegram.Bot.Types;
    using TelegramHandler.MessageProcessing;
    using TelegramHandler.StatesController;

    [TestFixture]
    public class AnswerProcessingTests
    {
        private readonly Mock<ITelegramBotClient> _telegramMock = new Mock<ITelegramBotClient>();
        private readonly Mock<ITestStateController> _testStateController = new Mock<ITestStateController>();
        private readonly Mock<ITasksStorage> _tasksStorage = new Mock<ITasksStorage>();
        private readonly IEnumerable<TaskInfo> _conclusiveMembersTasks = new List<TaskInfo>();
        private readonly IEnumerable<TaskInfo> _consultativeMembersTasks = new List<TaskInfo>();
        private readonly IEnumerable<TaskInfo> _observersTasks = new List<TaskInfo>();
        private readonly Mock<MessageEventArgs> _messageEventArgsMock = new Mock<MessageEventArgs>();
        private IMessageProcessing _testAnswerProcessing;

        [SetUp]
        public void SetUp()
        {
            _testStateController.Setup(c => c.IsUserTakingTest(It.IsAny<int>())).Returns(true);
            _tasksStorage.Setup(c => c.GetTasksForConclusiveMembers(It.IsAny<int>())).Returns(_conclusiveMembersTasks);
            _tasksStorage.Setup(c => c.GetTasksForConsultativeMembers(It.IsAny<int>())).Returns(_consultativeMembersTasks);
            _tasksStorage.Setup(c => c.GetTasksForObservers(It.IsAny<int>())).Returns(_observersTasks);
            _testAnswerProcessing = new TestAnswerProcessing(_telegramMock.Object, _testStateController.Object,
                _tasksStorage.Object, new TasksProcessing(), new StatisticsCollector());
        }

        [Test, Description(@"Аргументы события не подлежит обработке - сообщение равно нул.")]
        public void NullMessageTest()
        {
            _messageEventArgsMock.Setup(c => c.Message).Returns((PrivateClass) null);
            _testStateController.Setup(c => c.ResetState(It.IsAny<int>())).Throws(new IOException());
            Assert.DoesNotThrow(() => _testAnswerProcessing.OnMessage(null, _messageEventArgsMock.Object));
        }

        [Test, Description(@"Аргументы события не подлежит обработке - сообщение не является текстовым.")]
        public void NotTextMessageTest()
        {
            _messageEventArgsMock.Setup(c => c.Message).Returns(new PrivateClass());
            _testStateController.Setup(c => c.ResetState(It.IsAny<int>())).Throws(new IOException());
            Assert.DoesNotThrow(() => _testAnswerProcessing.OnMessage(null, _messageEventArgsMock.Object));
        }

        /// <summary>
        /// Класс для внутреннего использования.
        /// </summary>
        private class PrivateClass : Message
        {
        }
    }
}