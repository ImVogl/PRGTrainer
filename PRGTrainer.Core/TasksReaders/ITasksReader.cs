namespace PRGTrainer.Core.TasksReaders
{
    using System.Collections.Generic;
    using Model;

    /// <summary>
    /// Интерфейс ридера задач для членов комиссии с правом решающего голоса.
    /// </summary>
    public interface ITasksReader
    {
        /// <summary>
        /// Чтение коллекции задач из файла с задачами.
        /// </summary>
        /// <returns></returns>
        IEnumerable<Task> Read();
    }
}