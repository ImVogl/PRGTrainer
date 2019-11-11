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
        public IEnumerable<ReferenceBookPart> RootReferenceBookParts { get; private set; }

        /// <inheritdoc />
        public void FillStorage()
        {
            RootReferenceBookParts = _referenceBookReader.Read();
            _expandedTree.AddRange(RootReferenceBookParts.ToList());
            foreach (var part in RootReferenceBookParts)
                ExpandTree(part.SubParts);
        }

        /// <inheritdoc />
        public ReferenceBookPart GetPartById(int id)
        {
            return _expandedTree.Find(part => part.Identifier == id);
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