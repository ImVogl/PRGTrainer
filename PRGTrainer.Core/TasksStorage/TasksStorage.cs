namespace PRGTrainer.TasksStorage
{
    using System.Collections.Generic;
    using Model;
    using TasksReaders;

    /// <summary>
    /// Хранилище задач.
    /// </summary>
    public class TasksStorage : ITasksStorage
    {
        #region Private fields

        /// <summary>
        /// Ридер задач из файла с задачами.
        /// </summary>
        private readonly ITasksReader _tasksReader;

        #endregion
        
        /// <summary>
        /// Конструктор.
        /// </summary>
        /// <param name="reader">Ридер задач из файла с задачами.</param>
        public TasksStorage(ITasksReader reader)
        {
            _tasksReader = reader;
        }

        /// <inheritdoc />
        public IEnumerable<Task> Tasks { get; private set; }

        /// <inheritdoc />
        public void FillStorage()
        {
            Tasks = _tasksReader.Read();
        }
    }
}