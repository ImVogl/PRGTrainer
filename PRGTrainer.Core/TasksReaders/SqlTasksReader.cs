namespace PRGTrainer.Core.TasksReaders
{
    using System.Collections.Generic;
    using Model;

    /// <summary>
    /// Ридер задач из SQL базы данных.
    /// </summary>
    public class SqlTasksReader : ITasksReader
    {
        /// <inheritdoc />
        public IEnumerable<Task> Read()
        {
            throw new System.NotImplementedException();
        }
    }
}