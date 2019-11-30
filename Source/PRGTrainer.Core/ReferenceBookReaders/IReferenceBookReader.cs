namespace PRGTrainer.Core.ReferenceBookReaders
{
    using System.Collections.Generic;
    using Model.ReferenceBook;

    /// <summary>
    /// Интерфейс ридера справочника.
    /// </summary>
    public interface IReferenceBookReader
    {
        /// <summary>
        /// Чтение коллекции разделов справочника.
        /// </summary>
        /// <returns>Коллекция разделов справочника.</returns>
        IEnumerable<ReferenceBookPart> Read();
    }
}