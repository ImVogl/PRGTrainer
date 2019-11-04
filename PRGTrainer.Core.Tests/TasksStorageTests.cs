namespace PRGTrainer.Core.Tests
{
    using System.Collections.Generic;
    using System.Linq;
    using Model;
    using Moq;
    using NUnit.Framework;
    using TasksReaders;
    using TasksStorage;

    [TestFixture]
    public class TasksStorageTests
    {
        private readonly Mock<ITasksReader> _taskReader = new Mock<ITasksReader>();
        private ITasksStorage _tasksStorage;

        [SetUp]
        public void SetUp()
        {
            var taskCollection = new List<TaskInfo>
            {
                new TaskInfo { Question = @"Question 1" },
                new TaskInfo { Question = @"Question 2" },
                new TaskInfo { Question = @"Question 3" },
                new TaskInfo { Question = @"Question 4" }
            };

            _taskReader.Setup(c => c.Read()).Returns(taskCollection);
            _tasksStorage = new TasksStorage(_taskReader.Object);
            _tasksStorage.FillStorage();
        }

        [Test, Description(@"Число запрашиваемых задач больше числа задач в коллекции.")]
        public void InputCountGreatThanArrayCount()
        {
            var tasks = _tasksStorage.GetTasks(int.MaxValue).ToList();
            
            Assert.That(tasks.Count, Is.EqualTo(4));
        }

        [Test, Description(@"Задачи не должны повторяться.")]
        public void TaskShouldBeUnique()
        {
            var tasks = _tasksStorage.GetTasks(4).ToList();

            Assert.That(tasks.Count(c => c.Question == @"Question 1"), Is.EqualTo(1));
            Assert.That(tasks.Count(c => c.Question == @"Question 2"), Is.EqualTo(1));
            Assert.That(tasks.Count(c => c.Question == @"Question 3"), Is.EqualTo(1));
            Assert.That(tasks.Count(c => c.Question == @"Question 4"), Is.EqualTo(1));
        }
    }
}