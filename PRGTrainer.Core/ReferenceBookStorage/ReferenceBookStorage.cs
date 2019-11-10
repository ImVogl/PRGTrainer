namespace PRGTrainer.Core.ReferenceBookStorage
{
    using System.Collections.Generic;
    using Model.ReferenceBook;

    /// <summary>
    /// Хранилище справочника.
    /// </summary>
    public class ReferenceBookStorage : IReferenceBookStorage
    {
        #region Private fields

        /// <summary>
        /// Коллекция корневых разделов справочника.
        /// </summary>
        private IEnumerable<ReferenceBookPart> _rootReferenceBookParts;

        #endregion

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="ReferenceBookStorage"/>
        /// </summary>
        public ReferenceBookStorage()
        {
            _rootReferenceBookParts = new List<ReferenceBookPart>();
        }

        /// <inheritdoc />
        public void FillStorage()
        {
            throw new System.NotImplementedException();
        }
    }
}