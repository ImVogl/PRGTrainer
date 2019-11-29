namespace PRGTrainer.Core.TelegramHandler.Help.ParsingArguments
{
    using System.Collections.Generic;
    using JetBrains.Annotations;

    /// <summary>
    /// Интерфейс парсера аргументов.
    /// </summary>
    public interface IArgumentParser
    {
        /// <summary>
        /// Получает значение заданного параметра.
        /// </summary>
        /// <param name="arguments">Строка с аргументами.</param>
        /// <param name="paramName">Имя параметра.</param>
        /// <returns>Значение аргумента.</returns>
        [NotNull]
        string Parse([CanBeNull]string arguments, [NotNull]string paramName);

        /// <summary>
        /// Получает коллекция значений заданного параметра.
        /// </summary>
        /// <param name="arguments">Строка с аргументами.</param>
        /// <param name="paramName">Имя параметра.</param>
        /// <returns>Коллекция значений аргумента.</returns>
        [NotNull]
        IEnumerable<string> ParseCollection([CanBeNull]string arguments, [NotNull]string paramName);
    }
}