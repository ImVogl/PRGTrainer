namespace PRGTrainer.Core.AdminHandler
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using JetBrains.Annotations;

    /// <summary>
    /// Интерфейс работы администрирования.
    /// </summary>
    public interface IAdminHandler
    {
        /// <summary>
        /// Задает тип выходного файла со статистикой.
        /// </summary>
        StatisticOutputFileType OutputFileType { set; }

        /// <summary>
        /// Попытка добавить нового администратора.
        /// </summary>
        /// <param name="identifier">Идентификатор пользователя.</param>
        /// <param name="token">Токен.</param>
        /// <returns>Задача с результатом попытки добавления пользователя.</returns>
        Task<bool> TryAddNewAdmin([NotNull]int identifier, [NotNull] string token);

        /// <summary>
        /// Проверка, является ли пользователь администратором.
        /// </summary>
        /// <param name="identifier">Идентификатор пользователя.</param>
        /// <returns>Задача с результатом проверки.</returns>
        Task<bool> IsUserAdmin(int identifier);

        /// <summary>
        /// Получение статистики по пользователям.
        /// </summary>
        /// <param name="users">Коллекция пользователей.</param>
        /// <param name="startDate">Дата, начиная с которой стоит взять статистику.</param>
        /// <param name="identifier">Идентификатор пользователя.</param>
        /// <returns>Задание с путем до файла, где сохранена статистика.</returns>
        Task<string> GetStatisticForUsers([CanBeNull] ICollection<string> users, DateTime startDate, int identifier);

        /// <summary>
        /// Получение статистики по пользователям.
        /// </summary>
        /// <param name="users">Коллекция пользователей.</param>
        /// <param name="identifier">Идентификатор пользователя.</param>
        /// <returns>Задание с путем до файла, где сохранена статистика.</returns>
        Task<string> GetStatisticForUsers([CanBeNull] ICollection<string> users, int identifier);

        /// <summary>
        /// Получение статистики по вопросам.
        /// </summary>
        /// <param name="identifier">Идентификатор пользователя.</param>
        /// <returns>Задание с путем до файла, где сохранена статистика.</returns>
        Task<string> GetStatisticForQuestions(int identifier);
    }
}