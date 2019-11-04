namespace PRGTrainer.Core.Tests
{
    using System.Collections.Generic;
    using System.Linq;
    using Model;
    using NUnit.Framework;
    using TasksProcessing;

    [TestFixture]
    public class TasksProcessingTests
    {
        private const string First = @"first";
        private const string Second = @"second";
        private const string Third = @"third";
        private static readonly TaskInfo Task = new TaskInfo { Options = new List<string> { First, Second, Third }, CorrectOptionNum = 1 };

        private readonly ITasksProcessing _tasksProcessing = new TasksProcessing();

        [Test, Description(@"Проверка ответа в виде текста.")]
        public void CheckTextAnswerTest()
        {
            Assert.That(_tasksProcessing.IsAnswerCorrect(Task, First), Is.False);
            Assert.That(_tasksProcessing.IsAnswerCorrect(Task, Second), Is.True);
            Assert.That(_tasksProcessing.IsAnswerCorrect(Task, Third), Is.False);
        }

        [Test, Description(@"Проверка ответа по номеру правильного ответа.")]
        public void CheckNumAnswerTest()
        {
            Assert.That(_tasksProcessing.IsAnswerCorrect(Task, 0), Is.False);
            Assert.That(_tasksProcessing.IsAnswerCorrect(Task, 1), Is.True);
            Assert.That(_tasksProcessing.IsAnswerCorrect(Task, 2), Is.False);
        }

        [Test, Description(@"Проверка перемешивания задач.")]
        public void ShakeTest()
        {
            for (var i = 0; i < 10; i++)
            {
                var task = _tasksProcessing.Shake(Task);
                if (task.CorrectOptionNum == Task.CorrectOptionNum)
                    continue;

                if (i == 9)
                    throw new SuccessException(@"Не удалось перемешать варианты в задаче.");
                
                Assert.That(task.Options.ToList()[task.CorrectOptionNum], Is.EqualTo(Task.Options.ToList()[Task.CorrectOptionNum]));
                break;
            }
        }
    }
}