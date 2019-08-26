namespace PRGTrainer.TasksReaders
{
    using System.Collections.Generic;
    using Model;

    /// <summary>
    /// Ридер задач для членов комиссии с правом решающего голоса.
    /// </summary>
    public class PRGTasksReader : ITasksReader
    {
        #region
        
        /// <summary>
        /// Имя файла, содержащего коллекцию задач.
        /// </summary>
        private const string TaskFile = @"PRGTasks.xml";

        #endregion
        
        /// <inheritdoc />
        public IEnumerable<Task> Read()
        {
            throw new System.NotImplementedException();
        }
    }
}