﻿namespace PRGTrainer.TasksReaders
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Xml.Linq;
    using System.Xml.Schema;
    using Model;

    /// <summary>
    /// Ридер задач для членов комиссии с правом решающего голоса.
    /// </summary>
    public class PRGTasksReader : ITasksReader
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
        /// Текст сообщения при неправельной структуре файла.
        /// </summary>
        private const string IncorrectXmlStructureMsg = @"Неверная структура файла!";

        #endregion
        
        /// <inheritdoc />
        public IEnumerable<Task> Read()
        {
            var pathToTasksFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, TaskFile);
            var doc = XDocument.Load(pathToTasksFile);
            var taskElements = doc.Elements(TaskNodeName);

            return taskElements.Select(CreateTask);
        }

        /// <summary>
        /// Создает задачу из узла XML документа.
        /// </summary>
        /// <param name="node">Узел XML документа.</param>
        /// <returns></returns>
        private Task CreateTask(XElement node)
        {
            if (node.Element(@"correctAnswerId") == null)
                throw new XmlSchemaException(IncorrectXmlStructureMsg);

            if (node.Element(@"explanation") == null)
                throw new XmlSchemaException(IncorrectXmlStructureMsg);

            if (node.Element(@"question") == null)
                throw new XmlSchemaException(IncorrectXmlStructureMsg);

            if (node.Element(@"correctAnswerId") == null)
                throw new XmlSchemaException(IncorrectXmlStructureMsg);

            var answers =  node.Elements(@"answer").Select(c => c.Value).ToList();
            if (answers.Count != OptionsCount)
                throw new XmlSchemaException(IncorrectXmlStructureMsg);

            return new Task
            {
                CorrectOption = "",
                Explanation = @"",
                FirstWrongOption = "",
                SecondWrongOption = "",
                Qustion = ""
            };
        }
    }
}