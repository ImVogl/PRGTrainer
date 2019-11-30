namespace PRGTrainer.Core.Model.Result
{
    /// <summary>
    /// Результат по вопросам.
    /// </summary>
    public class QuestionResult
    {
        /// <summary>
        /// Вопрос.
        /// </summary>
        public string Question { get; set; }

        /// <summary>
        /// Доля неверного ответа от общего числа ответов.
        /// </summary>
        public double Quota { get; set; }
    }
}