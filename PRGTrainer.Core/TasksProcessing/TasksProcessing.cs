namespace PRGTrainer.Core.TasksProcessing
{
    using System;
    using System.Linq;
    using Model.Test;

    /// <summary>
    /// Обработчик задач.
    /// </summary>
    public class TasksProcessing : ITasksProcessing
    {
        /// <summary>
        /// Генератор случайных чисел.
        /// </summary>
        private readonly Random _random = new Random();

        /// <inheritdoc />
        public TaskInfo Shake(TaskInfo task)
        {
            var newTask = new TaskInfo
            {
                Question = task.Question,
                Explanation = task.Explanation,
                TargetMembers = task.TargetMembers
            };

            var correctAnswer = task.Options.ToList()[task.CorrectOptionNum];
            newTask.Options = task.Options.OrderBy(item => _random.Next());
            newTask.CorrectOptionNum = task.Options.ToList().IndexOf(correctAnswer);
            return newTask;
        }

        /// <inheritdoc />
        public bool IsAnswerCorrect(TaskInfo task, int num)
        {
            return task.CorrectOptionNum == num;
        }

        /// <inheritdoc />
        public bool IsAnswerCorrect(TaskInfo task, string answer)
        {
            return task.Options.ToList().IndexOf(answer) == task.CorrectOptionNum;
        }
    }
}