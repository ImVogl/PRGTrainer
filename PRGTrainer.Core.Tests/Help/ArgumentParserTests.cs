﻿namespace PRGTrainer.Core.Tests.Help
{
    using System.Linq;
    using NUnit.Framework;
    using TelegramHandler.Help.ParsingArguments;

    [TestFixture]
    public class ArgumentParserTests
    {
        private static readonly IArgumentParser ArgumentParser = new ArgumentParser();

        [Test, Description(@"Отсутствует нужный аргумент.")]
        public void NoTargetParam()
        {
            const string command = @"/aRg:value";
            Assert.That(ArgumentParser.Parse(command, @"/test"), Is.Empty);
        }

        [Test, Description(@"Получение аргумента.")]
        public void TargetArgument()
        {
            const string command = @"/first:first /aRg:vaLue, date /second:secondValue";
            Assert.That(ArgumentParser.Parse(command, @"/arg:"), Is.EqualTo(@"value, date"));
        }

        [Test, Description(@"Получение коллекции значений из аргумента.")]
        public void GetCollection()
        {
            const string command = @"/first:first /aRg:vaLue, date /second:secondValue";

            var result = ArgumentParser.ParseCollection(command, @"/arg:").ToList();
            Assert.That(result.Count, Is.EqualTo(2));
            Assert.That(result[0], Is.EqualTo(@"value"));
            Assert.That(result[1], Is.EqualTo(@"date"));
        }
    }
}