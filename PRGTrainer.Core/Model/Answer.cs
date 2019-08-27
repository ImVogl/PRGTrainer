namespace PRGTrainer.Core.Model
{
    /// <summary>
    /// Класс с ответом.
    /// </summary>
    public class Answer
    {
        /// <summary>
        /// Получает или задает идентификатор ответа.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Получает или задает содержимое ответа.
        /// </summary>
        public string Value { get; set; }
    }
}