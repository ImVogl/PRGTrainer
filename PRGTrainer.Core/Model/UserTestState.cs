namespace PRGTrainer.Core.Model
{
    using System.Collections.Generic;

    /// <summary>
    /// Плоский класс с информацией о состоянии теста для пользователя.
    /// </summary>
    public class UserTestState
    {
        /// <summary>
        /// Получает или задает имя пользователя.
        /// </summary>
        public string User { get; set; }

        /// <summary>
        /// Получает или задает коллекцию задач.
        /// </summary>
        public List<TaskInfo> TasksInfos { get; set; }

        /// <summary>
        /// Получает или задает номер текущей задачи.
        /// </summary>
        public int CurrentTaskNum { get; set; }

        /// <summary>
        /// Получает или задает состояние в рамках теста.
        /// </summary>
        public string SubState { get; set; }

        /// <summary>
        /// Получает или задает коллекцию результатов проверок ответов пользователя.
        /// </summary>
        public List<bool> Results { get; set; }
    }
}