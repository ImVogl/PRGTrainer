namespace PRGTrainer.Core.ReferenceBookStorage
{
    using System.Collections.Generic;
    using System.Linq;
    using Model.ReferenceBook;
    using ReferenceBookReaders;

    /// <summary>
    /// Хранилище справочника.
    /// </summary>
    public class ReferenceBookStorage : IReferenceBookStorage
    {
        /// <summary>
        /// Ридер справочника.
        /// </summary>
        private readonly IReferenceBookReader _referenceBookReader;

        /// <summary>
        /// Развернутое дерево 
        /// </summary>
        private readonly List<ReferenceBookPart> _expandedTree;

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="ReferenceBookStorage"/>
        /// </summary>
        /// <param name="referenceBookReader">Ридер справочника.</param>
        public ReferenceBookStorage(IReferenceBookReader referenceBookReader)
        {
            _referenceBookReader = referenceBookReader;
            _expandedTree = new List<ReferenceBookPart>();
        }

        /// <inheritdoc />
        public ReferenceBookPart RootReferenceBookParts { get; private set; }

        /// <inheritdoc />
        public void FillStorage()
        {
            var referenceBook = _referenceBookReader.Read().ToList();
            RootReferenceBookParts = new ReferenceBookPart { SubParts = referenceBook, Identifier = 0, Name = @"Справочник", ParentIdentifier = -1 };

            _expandedTree.AddRange(referenceBook);
            foreach (var part in referenceBook)
                ExpandTree(part.SubParts);
        }

        /// <inheritdoc />
        public ReferenceBookPart GetPartById(int id)
        {
            return id == 0 ? RootReferenceBookParts : _expandedTree.First(part => part.Identifier == id);
        }

        /// <summary>
        /// Разворачивание дерева справочника.
        /// </summary>
        /// <param name="parts">Коллекция разделов.</param>
        private void ExpandTree(IEnumerable<ReferenceBookPart> parts)
        {
            _expandedTree.AddRange(parts);
            foreach (var part in parts)
                ExpandTree(part.SubParts);
        }
    }
}