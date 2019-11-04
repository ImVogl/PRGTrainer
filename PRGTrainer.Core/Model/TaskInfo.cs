namespace PRGTrainer.Core.Model
{
    using System.Collections.Generic;

    /// <summary>
    /// Плоский класс, содержащий сведения о задаче.
    /// </summary>
    public class TaskInfo
    {
        /// <summary>
        /// Получает или задает вопрос.
        /// </summary>
        public string Question { get; set; }

        /// <summary>
        /// Получает или задает коллекцию вариантов ответа.
        /// </summary>
        public IEnumerable<string> Options { get; set; }

        /// <summary>
        /// Получает или задает номер корректного ответа.
        /// </summary>
        public int CorrectOptionNum { get; set; }

        /// <summary>
        /// Получает или задает пояснения к ответу на вопрос.
        /// </summary>
        public string Explanation { get; set; }

        /// <summary>
        /// Получает или задает тип участника, проходящего тест.
        /// </summary>
        public IEnumerable<MemberType> TargetMembers { get; set; }
    }
}