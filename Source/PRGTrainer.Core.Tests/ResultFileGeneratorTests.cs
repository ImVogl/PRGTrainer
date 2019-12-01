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
            var content = new Bitmap(path);
            var expectedContent = new Bitmap(Path.Combine(WorkFolder, @"UsersResult.png"));

            Assert.That(CompareImages(content, expectedContent), Is.True);

            expectedContent.Dispose();
            content.Dispose();
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

        [Test, Description(@"Получение результатов по вопросам в виде изображения.")]
        public void GenerateImgResultQuestionTest()
        {
            var questionResults = new List<QuestionResult>
            {
                new QuestionResult { Quota = 0.3, Question = @"Вопрос 1" },
                new QuestionResult { Quota = 0.4, Question = @"Вопрос 2" },
                new QuestionResult { Quota = 0.2, Question = @"Вопрос 3" },
                new QuestionResult { Quota = 0.1, Question = @"Вопрос 4" }
            };

            var path = _resultFileGenerator.GenerateAsImage(questionResults);
            var content = new Bitmap(path);
            var expectedContent = new Bitmap(Path.Combine(WorkFolder, @"QuestionResult.png"));

            Assert.That(CompareImages(content, expectedContent), Is.True);

            expectedContent.Dispose();
            content.Dispose();
        }

        public static bool CompareImages(Bitmap image1, Bitmap image2)
        {
            var flag = true;
            var width = Math.Min(image1.Width, image2.Width);
            var height = Math.Min(image1.Height, image2.Height);
            var bitmap = new Bitmap(width, height);
            var white = Color.White;
            var red = Color.Red;
            for (var x = 0; x < width; ++x)
            {
                for (var y = 0; y < height; ++y)
                {
                    if (image1.GetPixel(x, y).Equals((object)image2.GetPixel(x, y)))
                    {
                        bitmap.SetPixel(x, y, white);
                    }
                    else
                    {
                        bitmap.SetPixel(x, y, red);
                        flag = false;
                    }
                }
            }

            return flag;
        }
    }
}