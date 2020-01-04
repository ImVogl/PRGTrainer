namespace SignExtractor.Canvas
{
    using System;
    using System.Drawing;

    /// <summary>
    /// Интерфейс презентера области прорисовки изображения.
    /// </summary>
    public interface ICanvasPresenter : IDisposable
    {
        /// <summary>
        /// Получает положение признака.
        /// </summary>
        Rectangle SignPosition { get; }

        /// <summary>
        /// Задание текущего масштаба изображения.
        /// </summary>
        /// <param name="progress">Доля от максимального масштаба.</param>
        void SetImageScale(float progress);
    }
}