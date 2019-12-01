namespace PRGTrainer.Core.ReferenceBookStorage
{
    using System.Collections.Generic;
    using Model.ReferenceBook;

    /// <summary>
    /// Интерфейс справочника.
    /// </summary>
    public interface IReferenceBookStorage
    {
        /// <summary>
        /// Получает корневой раздел справочника.
        /// </summary>
        ReferenceBookPart RootReferenceBookParts { get; }

        /// <summary>
        /// Заполнение хранилища.
        /// </summary>
        void FillStorage();

        /// <summary>
        /// Получение раздела справочника по идентификатору.
        /// </summary>
        /// <param name="id">Идентификатор.</param>
        /// <returns>Раздел справочника.</returns>
        ReferenceBookPart GetPartById(int id);
    }
}