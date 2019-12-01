namespace PRGTrainer.Core.TelegramHandler.Help.ParsingArguments
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;

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

            var regex = new Regex(paramName, RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);
            var secondPart = regex.Split(arguments, 2).LastOrDefault() ?? string.Empty;

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