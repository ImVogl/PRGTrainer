namespace PRGTrainer.Core.TasksStorage
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
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

        /// <summary>
        /// Коллекция задач.
        /// </summary>
        private List<Task> _tasks;

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
        public IEnumerable<Task> GetTasks(int num)
        {
            var randomList = new List<Task>();
            var random = new Random();
            while (_tasks.Count > 0)
            {
                var randomIndex = random.Next(0, _tasks.Count);
                randomList.Add(_tasks[randomIndex]);
                _tasks.RemoveAt(randomIndex);
            }

            return randomList.Take(num);
        }

        /// <inheritdoc />
        public void FillStorage()
        {
            _tasks = _tasksReader.Read().ToList();
        }
    }
}