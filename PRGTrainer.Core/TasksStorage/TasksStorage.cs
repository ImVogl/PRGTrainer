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
        private List<TaskInfo> _tasks;

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
        public IEnumerable<TaskInfo> GetTasksForConclusiveMembers(int num)
        {
            return GetTasks(num, MemberType.Conclusive);
        }

        /// <inheritdoc />
        public IEnumerable<TaskInfo> GetTasksForConsultativeMembers(int num)
        {
            return GetTasks(num, MemberType.Consultative);
        }

        /// <inheritdoc />
        public IEnumerable<TaskInfo> GetTasksForObservers(int num)
        {
            return GetTasks(num, MemberType.Observer);
        }

        /// <inheritdoc />
        public void FillStorage()
        {
            _tasks = _tasksReader.Read().ToList();
        }

        /// <summary>
        /// Получение коллекции задач.
        /// </summary>
        /// <param name="num">Число задач.</param>
        /// <param name="type">Тип задач.</param>
        /// <returns>Коллекция задач.</returns>
        private IEnumerable<TaskInfo> GetTasks(int num, MemberType type)
        {
            var randomList = new List<TaskInfo>();
            var tempTasksCollection = new List<TaskInfo>(_tasks.Where(c => c.TargetMembers.Contains(type)));
            var random = new Random();
            while (tempTasksCollection.Count > 0)
            {
                var randomIndex = random.Next(0, _tasks.Count);
                randomList.Add(_tasks[randomIndex]);
                tempTasksCollection.RemoveAt(randomIndex);
            }

            return randomList.Take(num);
        }
    }
}