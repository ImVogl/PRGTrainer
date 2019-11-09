namespace PRGTrainer.Core.TasksStorage
{
    using System.Collections.Generic;
    using Model.Test;

    /// <summary>
    /// Интерфейс хранилища вопросов.
    /// </summary>
    public interface ITasksStorage
    {
        /// <summary>
        /// Получение списка задач для членов комиссии с правом решающего голоса.
        /// </summary>
        /// <param name="num">Число задач, которые необходимо отдать.</param>
        /// <returns>Коллекция задач.</returns>
        IEnumerable<TaskInfo> GetTasksForConclusiveMembers(int num);

        /// <summary>
        /// Получение списка задач для членов комиссии с правом совещательного голоса.
        /// </summary>
        /// <param name="num">Число задач, которые необходимо отдать.</param>
        /// <returns>Коллекция задач.</returns>
        IEnumerable<TaskInfo> GetTasksForConsultativeMembers(int num);

        /// <summary>
        /// Получение списка задач для наблюдателей.
        /// </summary>
        /// <param name="num">Число задач, которые необходимо отдать.</param>
        /// <returns>Коллекция задач.</returns>
        IEnumerable<TaskInfo> GetTasksForObservers(int num);

        /// <summary>
        /// Заполнение хранилища задач.
        /// </summary>
        void FillStorage();
    }
}