namespace PRGTrainer.Core.TasksReaders
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Xml.Linq;
    using System.Xml.Schema;
    using Model.Test;

    /// <summary>
    /// Ридер задач из файла.
    /// </summary>
    public class FileTasksReader : ITasksReader
    {
        #region Private fields
        
        /// <summary>
        /// Имя файла, содержащего коллекцию задач.
        /// </summary>
        private const string TaskFile = @"PRGTasks.xml";

        /// <summary>
        /// Имя узла, в котором хранится каждая из задач.
        /// </summary>
        private const string TaskNodeName = @"task";

        /// <summary>
        /// Число вариантов ответа.
        /// </summary>
        private const byte OptionsCount = 3;

        /// <summary>
        /// Текст сообщения при неправильной структуре файла.
        /// </summary>
        private const string IncorrectXmlStructureMsg = @"Неверная структура файла!";

        /// <summary>
        /// Значение узла, свидетельствующее о том, что задача подходит членам комиссии с правом решающего голоса.
        /// </summary>
        private const string ConclusiveMember = @"Conclusive";

        /// <summary>
        /// Значение узла, свидетельствующее о том, что задача подходит членам комиссии с совещательного решающего голоса.
        /// </summary>
        private const string ConsultativeMember = @"Consultative";

        /// <summary>
        /// Значение узла, свидетельствующее о том, что задача подходит наблюдателям.
        /// </summary>
        private const string Observer = @"Observer";

        #endregion
        
        /// <inheritdoc />
        public IEnumerable<TaskInfo> Read()
        {
            var pathToTasksFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, TaskFile);
            var doc = XDocument.Load(pathToTasksFile);
            var taskElements = doc.Descendants(TaskNodeName).ToList();

            return taskElements.Select(CreateTask);
        }

        /// <summary>
        /// Создает задачу из узла XML документа.
        /// </summary>
        /// <param name="node">Узел XML документа.</param>
        /// <returns></returns>
        private TaskInfo CreateTask(XElement node)
        {
            if (node.Element(@"correctAnswerId") == null)
                throw new XmlSchemaException(IncorrectXmlStructureMsg);

            if (node.Element(@"explanation") == null)
                throw new XmlSchemaException(IncorrectXmlStructureMsg);

            if (node.Element(@"question") == null)
                throw new XmlSchemaException(IncorrectXmlStructureMsg);

            if (node.Element(@"memberTypes") == null)
                throw new XmlSchemaException(IncorrectXmlStructureMsg);

            var answers =  node.Elements(@"answer")
                .Select(c => new Answer { Id = c.Attribute("id").Value, Value = c.Value })
                .ToList();

            if (answers.Count != OptionsCount)
                throw new XmlSchemaException(IncorrectXmlStructureMsg);

            int correctId;
            if (!int.TryParse(node.Element(@"correctAnswerId").Value, out correctId))
                throw new XmlSchemaException(@"Не удалось прочесть идентификатор верного варианта ответа!");
            
            return new TaskInfo
            {
                CorrectOptionNum = correctId - 1,
                Options = new List<string>
                {
                    answers.Single(c => c.Id == @"1").Value,
                    answers.Single(c => c.Id == @"2").Value,
                    answers.Single(c => c.Id == @"3").Value
                },

                Explanation = node.Element(@"explanation").Value,
                Question = node.Element(@"question").Value,
                TargetMembers = GetTargetMembers(node.Element(@"memberTypes").Value)
            };
        }

        /// <summary>
        /// Получает коллекцию участников избирательного процесса, для которых доступен вопрос.
        /// </summary>
        /// <param name="memberTypes">Строка с типами участников избирательного процесса.</param>
        /// <returns>Коллекция типов участников избирательного процесса.</returns>
        private static IEnumerable<MemberType> GetTargetMembers(string memberTypes)
        {
            var typeCollections = new List<MemberType>();
            var parsedTypes = memberTypes.Split(',').Select(c => c.Trim(' ')).ToList();
            if (parsedTypes.Any(c => c == ConclusiveMember))
                typeCollections.Add(MemberType.Conclusive);

            if (parsedTypes.Any(c => c == ConsultativeMember))
                typeCollections.Add(MemberType.Consultative);

            if(parsedTypes.Any(c => c == Observer))
                typeCollections.Add(MemberType.Observer);

            return typeCollections;
        }
    }
}