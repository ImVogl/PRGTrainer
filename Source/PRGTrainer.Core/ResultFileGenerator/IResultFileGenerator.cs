namespace PRGTrainer.Core.ResultFileGenerator
{
    using System.Collections.Generic;
    using JetBrains.Annotations;
    using Model.Result;

    /// <summary>
    /// Интерфейс генератора результирующих файлов.
    /// </summary>
    public interface IResultFileGenerator
    {
        /// <summary>
        /// Получение результата в виде текстового файла для вопросов.
        /// </summary>
        /// <param name="questionResults">Результаты по вопросам.</param>
        /// <returns>Путь до файла с результатами.</returns>
        [NotNull]
        string GenerateAsText(IEnumerable<QuestionResult> questionResults);

        /// <summary>
        /// Получение результата в виде текстового файла для пользователей.
        /// </summary>
        /// <param name="userResults">Результаты по пользователям.</param>
        /// <returns>Путь до файла с результатами.</returns>
        [NotNull]
        string GenerateAsText(IEnumerable<UserResult> userResults);

        /// <summary>
        /// Получение результата в виде изображения для вопросов.
        /// </summary>
        /// <param name="questionResults">Результаты по вопросам.</param>
        /// <returns>Путь до файла с результатами.</returns>
        string GenerateAsImage(IEnumerable<QuestionResult> questionResults);

        /// <summary>
        /// Получение результата в виде изображения для пользователей.
        /// </summary>
        /// <param name="userResults">Результаты по пользователям.</param>
        /// <returns>Путь до файла с результатами.</returns>
        [NotNull]
        string GenerateAsImage(IEnumerable<UserResult> userResults);
    }
}