namespace PRGTrainer.Core.TelegramHandler.Help.ParsingArguments
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Парсер аргументов.
    /// </summary>
    public class ArgumentParser : IArgumentParser
    {
        /// <inheritdoc />
        public string Parse(string arguments, string paramName)
        {
            if (string.IsNullOrWhiteSpace(arguments) || !arguments.ToLower().Contains(paramName.ToLower()))
                return string.Empty;

            var secondPart = arguments
                .ToLower()
                .Split(new[] { paramName.ToLower() }, StringSplitOptions.RemoveEmptyEntries)
                .LastOrDefault() ?? string.Empty;

            return !string.IsNullOrWhiteSpace(secondPart) 
                ? secondPart.Split('/').First().TrimEnd(' ')
                : secondPart;
        }

        /// <inheritdoc />
        public IEnumerable<string> ParseCollection(string arguments, string paramName)
        {
            return Parse(arguments, paramName)
                .Split(' ')
                .Select(item => item.TrimEnd(',').TrimStart(','));
        }
    }
}