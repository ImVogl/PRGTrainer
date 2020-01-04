namespace SignExtractor.MainHost
{
    /// <summary>
    /// Интерфейс презентера основного отображения.
    /// </summary>
    public interface IMainHostPresenter
    {
        /// <summary>
        /// Получает значение, показывающее, что коллекция обрабатываемых изображений пуста.
        /// </summary>
        bool IsImagesListEmpty { get; }
        
        /// <summary>
        /// Действие при нажатии на 
        /// </summary>
        void SetImages();

        /// <summary>
        /// Задание предыдущего отображения.
        /// </summary>
        /// <returns>Значение, показывающее, что контрол не является первым.</returns>
        bool SetPreviousCanvas();

        /// <summary>
        /// Задание следующего отображения.
        /// </summary>
        /// <returns>Значение, показывающее, что контрол не является последним.</returns>
        bool SetNextCanvas();

        /// <summary>
        /// Удаление текущего изображения.
        /// </summary>
        /// <returns>Значение, показывающее, что контрол не является последним.</returns>
        bool RemoveCanvas();

        /// <summary>
        /// Сохраняет расположение признаков.
        /// </summary>
        void SaveSignPositions();
    }
}