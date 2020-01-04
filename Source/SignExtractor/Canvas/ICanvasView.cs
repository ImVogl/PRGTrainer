namespace SignExtractor.Canvas
{
    using System.Drawing;

    /// <summary>
    /// Интерфейс отображения области прорисовки изображения.
    /// </summary>
    public interface ICanvasView
    {
        /// <summary>
        /// Получает область с признаком.
        /// </summary>
        Rectangle Area { get; }

        /// <summary>
        /// Задание текущего масштаба изображения.
        /// </summary>
        /// <param name="progress">Доля от максимального масштаба.</param>
        void SetImageScale(float progress);
    }
}