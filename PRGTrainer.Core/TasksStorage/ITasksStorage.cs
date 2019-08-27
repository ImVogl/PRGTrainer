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
        /// Получает коллекцию задач.
        /// </summary>
        IEnumerable<Task> Tasks { get; }

        /// <summary>
        /// Заполнение хранилища задач.
        /// </summary>
        void FillStorage();
    }
}