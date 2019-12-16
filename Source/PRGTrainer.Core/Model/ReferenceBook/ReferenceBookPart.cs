namespace PRGTrainer.Core.Model.ReferenceBook
{
    using System.Collections.Generic;
    using JetBrains.Annotations;

    /// <summary>
    /// Раздел справочника.
    /// </summary>
    public class ReferenceBookPart
    {
        /// <summary>
        /// Получает или задает идентификатор раздела.
        /// </summary>
        [NotNull]
        public int Identifier { get; set; }

        /// <summary>
        /// Получает или задает идентификатор родительского раздела.
        /// </summary>
        public int ParentIdentifier { get; set; }

        /// <summary>
        /// Получает или задает имя раздела.
        /// </summary>
        [NotNull]
        public string Name { get; set; }

        /// <summary>
        /// Получает или задает содержимое раздела.
        /// </summary>
        [CanBeNull]
        public string Content { get; set; }

        /// <summary>
        /// Получает или задает путь до файла.
        /// </summary>
        [CanBeNull]
        public string FilePath { get; set; }

        /// <summary>
        /// Получает или задает коллекцию подразделов.
        /// </summary>
        [CanBeNull]
        public IEnumerable<ReferenceBookPart> SubParts { get; set; }
    }
}