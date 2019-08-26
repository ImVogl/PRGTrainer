namespace PRGTrainer.TasksStorage
{
    using System.Collections.Generic;
    using Model;

    /// <summary>
    /// Хранилище задач.
    /// </summary>
    public class TasksStorage : ITasksStorage
    {
        /// <inheritdoc />
        public IEnumerable<Task> Tasks { get; private set; }

        /// <inheritdoc />
        public void FillStorage()
        {
            throw new System.NotImplementedException();
        }
    }
}