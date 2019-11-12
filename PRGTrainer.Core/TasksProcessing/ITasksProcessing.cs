namespace PRGTrainer.Core.TasksProcessing
{
    using Model.Test;

    /// <summary>
    /// Интерфейс обработчика задач.
    /// </summary>
    public interface ITasksProcessing
    {
        /// <summary>
        /// Перемешать вопросы.
        /// </summary>
        /// <param name="task">Задача.</param>
        /// <returns>Задача с переставленным правильным вариантом ответа.</returns>
        TaskInfo Shake(TaskInfo task);

        /// <summary>
        /// Проверяет верно ли дан ответ на вопрос в задаче.
        /// </summary>
        /// <param name="task">Задача.</param>
        /// <param name="num">Номер ответа.</param>
        /// <returns>Значение, показывающее верен ли ответ.</returns>
        bool IsAnswerCorrect(TaskInfo task, int num);

        /// <summary>
        /// Проверяет верно ли дан ответ на вопрос в задаче.
        /// </summary>
        /// <param name="task">Задача.</param>
        /// <param name="answer">Ответ.</param>
        /// <returns>Значение, показывающее верен ли ответ.</returns>
        bool IsAnswerCorrect(TaskInfo task, string answer);
    }
}