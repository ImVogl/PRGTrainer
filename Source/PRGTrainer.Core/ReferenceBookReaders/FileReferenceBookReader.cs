namespace PRGTrainer.Core.ReferenceBookReaders
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Xml.Linq;
    using System.Xml.Schema;
    using Model.ReferenceBook;

    /// <summary>
    /// Ридер справочника из файла.
    /// </summary>
    public class FileReferenceBookReader : IReferenceBookReader
    {
        #region Private fields

        /// <summary>
        /// Имя файла, где хранится справочник.
        /// </summary>
        private const string ReferenceBookFile = @"ReferenceBook.xml";

        /// <summary>
        /// Имя узла с разделом справочника.
        /// </summary>
        private const string PartNode = @"part";

        /// <summary>
        /// Имя узла с подразделом.
        /// </summary>
        private const string SubPartNode = @"subpart";

        /// <summary>
        /// Имя узла с содержимым раздела.
        /// </summary>
        private const string Content = @"content";

        /// <summary>
        /// Имя узла с относительным путем до файла.
        /// </summary>
        private const string FilePath = @"path";

        /// <summary>
        /// Счетчик идентификаторов.
        /// </summary>
        private int _identifierCount;

        #endregion
        
        /// <inheritdoc />
        public IEnumerable<ReferenceBookPart> Read()
        {
            var pathToTasksFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ReferenceBookFile);
            var doc = XDocument.Load(pathToTasksFile);
            var taskElements = doc.Descendants(PartNode).ToList();

            return taskElements.Select(element => ResolveStructure(element, 0)).ToList();
        }

        /// <summary>
        /// Получает раздел справочника с его содержимым и зависимостями.
        /// </summary>
        /// <param name="node">Узел</param>
        /// <param name="parentIdentifier">Идентификатор родительского раздела.</param>
        /// <returns>Секция справочника.</returns>
        private ReferenceBookPart ResolveStructure(XElement node, int parentIdentifier)
        {
            if (node.Element(SubPartNode) == null && node.Element(Content) == null)
                throw new XmlSchemaException(@"Раздел не имеет ни содержимого ни дочерних узлов.");

            if (node.Attribute("Name") == null || string.IsNullOrWhiteSpace(node.Attribute("Name").Value))
                throw new XmlSchemaException(@"Не удалось получить имя раздела.");

            var assemblyPath = Assembly.GetAssembly(typeof(FileReferenceBookReader)).Location;
            var filePath = Path.Combine(Path.GetDirectoryName(assemblyPath) ?? string.Empty, node.Element(FilePath)?.Value ?? string.Empty);

            _identifierCount++;
            var localIdentifier = _identifierCount;
            return new ReferenceBookPart
            {
                Name = node.Attribute("Name").Value,
                Identifier = localIdentifier,
                Content = node.Element(Content)?.Value,
                FilePath = File.Exists(filePath) ? filePath : null,
                ParentIdentifier = parentIdentifier,
                SubParts = node.Elements(SubPartNode).Select(element => ResolveStructure(element, localIdentifier)).ToList()
            };
        }
    }
}