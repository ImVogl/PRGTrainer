namespace PRGTrainer.Core.TasksStorage
{
    using System.Collections.Generic;
    using Model;

    /// <summary>
    /// Интерфейс хранилища вопросов.
    /// </summary>
    public interface ITasksStorage
    {
        /// <summary>
        /// Получение коллекции задач.
        /// </summary>
        /// <param name="num">Число задач, которые необходимо отдать.</param>
        /// <returns>Коллекция задач.</returns>
        IEnumerable<Task> GetTasks(int num);

        /// <summary>
        /// Заполнение хранилища задач.
        /// </summary>
        void FillStorage();
    }
}