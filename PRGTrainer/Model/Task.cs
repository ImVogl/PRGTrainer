namespace PRGTrainer.Model
{
    /// <summary>
    /// Плоский класс, содержищий сведения о задаче.
    /// </summary>
    public class Task
    {
        /// <summary>
        /// Получает или задает вопрос.
        /// </summary>
        public string Qustion { get; set; }

        /// <summary>
        /// Получает или задает первый неправильный вариант ответа.
        /// </summary>
        public string FirstWrongOption { get; set; }

        /// <summary>
        /// Получает или задает второй неправильный вариант ответа.
        /// </summary>
        public string SecondWrongOption { get; set; }

        /// <summary>
        /// Получает или задает правильный вариант ответа.
        /// </summary>
        public string CorrectOption { get; set; }

        /// <summary>
        /// Получает или задает пояснения к ответу на вопрос.
        /// </summary>
        public string Explanation { get; set; }
    }
}