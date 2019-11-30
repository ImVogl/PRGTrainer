namespace PRGTrainer.Core.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.IO;
    using Model.Result;
    using NUnit.Framework;
    using ResultFileGenerator;

    [TestFixture]
    public class ResultFileGeneratorTests
    {
        private static readonly string WorkFolder = Path.Combine(TestContext.CurrentContext.TestDirectory, @"TestItems", @"ResultFileGenerator");
        private readonly IResultFileGenerator _resultFileGenerator = new ResultFileGenerator();

        [TearDown]
        public void TearDown()
        {
            var files = Directory.GetFiles(WorkFolder, @"*", SearchOption.TopDirectoryOnly);
            foreach (var file in files)
                File.Delete(file);
        }

        [Test, Description(@"Получение результатов по пользователям.")]
        public void GenerateTxtResultUserTest()
        {
            var userResults = new List<UserResult>
            {
                new UserResult { User = @"First", Result = new Dictionary<DateTime, int> { { new DateTime(2010, 12, 15), 50 }, { new DateTime(2013, 10, 5), 90 } } },
                new UserResult { User = @"Second", Result = new Dictionary<DateTime, int> { { new DateTime(2010, 11, 13), 35 }, { new DateTime(2012, 6, 25), 15 } } }
            };

            var path = _resultFileGenerator.GenerateAsText(userResults);
            var content = File.ReadAllLines(path);
            var expectedContent = File.ReadAllLines(Path.Combine(WorkFolder, @"UsersResult.txt"));

            Assert.That(content, Is.EqualTo(expectedContent));
        }

        [Test, Description(@"Получение результатов по пользователям в виде изображения.")]
        public void GenerateImgResultUserTest()
        {
            var userResults = new List<UserResult>
            {
                new UserResult { User = @"First", Result = new Dictionary<DateTime, int> { { new DateTime(2010, 12, 15), 50 }, { new DateTime(2013, 10, 5), 90 } } },
                new UserResult { User = @"Second", Result = new Dictionary<DateTime, int> { { new DateTime(2010, 11, 13), 35 }, { new DateTime(2012, 6, 25), 15 } } }
            };

            var path = _resultFileGenerator.GenerateAsImage(userResults);
            var content = Image.FromFile(path);
            var expectedContent = Image.FromFile(Path.Combine(WorkFolder, @"UsersResult.png"));

            Assert.That(content, Is.EqualTo(expectedContent));
        }

        [Test, Description(@"Получение результатов по вопросам.")]
        public void GenerateTxtResultQuestionTest()
        {
            var questionResults = new List<QuestionResult>
            {
                new QuestionResult { Quota = 0.3, Question = @"Вопрос 1" },
                new QuestionResult { Quota = 0.4, Question = @"Вопрос 2" },
                new QuestionResult { Quota = 0.2, Question = @"Вопрос 3" },
                new QuestionResult { Quota = 0.1, Question = @"Вопрос 4" }
            };

            var path = _resultFileGenerator.GenerateAsText(questionResults);
            var content = File.ReadAllLines(path);
            var expectedContent = File.ReadAllLines(Path.Combine(WorkFolder, @"QuestionResult.txt"));

            Assert.That(content, Is.EqualTo(expectedContent));
        }

        private bool AreImagesSame(string firstPath, string secondPath)
        {
            //
        }
    }
}