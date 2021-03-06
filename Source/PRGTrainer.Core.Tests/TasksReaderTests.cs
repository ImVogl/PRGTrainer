﻿namespace PRGTrainer.Core.Tests
{
    using System.IO;
    using System.Linq;
    using System.Xml.Schema;
    using Model.Test;
    using NUnit.Framework;
    using TasksReaders;

    [TestFixture]
    public class TasksReaderTests
    {
        private static readonly string TestFolder = TestContext.CurrentContext.TestDirectory;
        private readonly ITasksReader _tasksReader = new FileTasksReader();

        [Test, Description(@"Получение коллекции задач.")]
        public void GetTasks()
        {
            CopyTargetBase(@"Correct");
            var tasks = _tasksReader.Read().ToList();

            Assert.That(tasks, Is.Not.Null);
            Assert.That(tasks.Count, Is.EqualTo(2));

            var firstTask = tasks.First();
            Assert.That(firstTask.Options.ToList()[1], Is.EqualTo(@"Correct 1."));
            Assert.That(firstTask.Options.ToList()[0], Is.EqualTo(@"Incorrect 1.1."));
            Assert.That(firstTask.Options.ToList()[2], Is.EqualTo(@"Incorrect 1.2."));
            Assert.That(firstTask.Question, Is.EqualTo(@"Question 1"));
            Assert.That(firstTask.Explanation, Is.EqualTo(@"Explanation 1."));
            Assert.That(firstTask.TargetMembers.Count(), Is.EqualTo(3));
            Assert.That(firstTask.TargetMembers.Contains(MemberType.Conclusive));
            Assert.That(firstTask.TargetMembers.Contains(MemberType.Conclusive));
            Assert.That(firstTask.TargetMembers.Contains(MemberType.Observer));

            var secondTask = tasks.Last();
            Assert.That(secondTask.Options.ToList()[0], Is.EqualTo(@"Correct 2."));
            Assert.That(secondTask.Options.ToList()[1], Is.EqualTo(@"Incorrect 2.1."));
            Assert.That(secondTask.Options.ToList()[2], Is.EqualTo(@"Incorrect 2.2."));
            Assert.That(secondTask.Question, Is.EqualTo(@"Question 2"));
            Assert.That(secondTask.Explanation, Is.EqualTo(@"Explanation 2."));
            Assert.That(secondTask.TargetMembers.Count(), Is.EqualTo(1));
            Assert.That(secondTask.TargetMembers.Contains(MemberType.Conclusive));
        }

        [Test, Description(@"Отсутствует узел с вопросом.")]
        public void NoQuestionExceptionTest()
        {
            CopyTargetBase(@"NoQuestion");
            Assert.Throws(typeof(XmlSchemaException), () => _tasksReader.Read().ToList());
        }

        [Test, Description(@"Число вариантов ответа не равно трем.")]
        public void IncorrectAnswersCountExceptionTest()
        {
            CopyTargetBase(@"IncorrectAnswersCount");
            Assert.Throws(typeof(XmlSchemaException), () => _tasksReader.Read().ToList());
        }

        [Test, Description(@"Отсутствует узел с идентификатором правильного ответа.")]
        public void NoCorrectIdExceptionTest()
        {
            CopyTargetBase(@"NoAnswerId");
            Assert.Throws(typeof(XmlSchemaException), () => _tasksReader.Read().ToList());
        }

        [Test, Description(@"Отсутствует узел с объяснением правильного ответа.")]
        public void NoExplanationExceptionTest()
        {
            CopyTargetBase(@"NoExplanation");
            Assert.Throws(typeof(XmlSchemaException), () => _tasksReader.Read().ToList());
        }

        [Test, Description(@"Отсутствует тип пользователей.")]
        public void NoMemberType()
        {
            CopyTargetBase(@"NoMemberType");
            Assert.Throws(typeof(XmlSchemaException), () => _tasksReader.Read().ToList());
        }

        private void CopyTargetBase(string sourceFolderName)
        {
            var targetPath = Path.Combine(TestFolder, @"PRGTasks.xml");
            if (File.Exists(targetPath))
                File.Delete(targetPath);

            var sourcePath = Path.Combine(TestFolder, @"TestItems", sourceFolderName, @"PRGTasks.xml");
            File.Copy(sourcePath, targetPath);
        }
    }
}
