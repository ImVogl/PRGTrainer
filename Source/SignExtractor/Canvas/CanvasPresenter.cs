namespace SignExtractor.Canvas
{
    using System;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Windows.Forms;

    /// <summary>
    /// Презентер области прорисовки изображения.
    /// </summary>
    public class CanvasPresenter
    {
        #region Private fields

        /// <summary>
        /// Толщина линии.
        /// </summary>
        private const float LineWidth = 2;

        /// <summary>
        /// Прорисовщик изображений.
        /// </summary>
        private readonly Pen _pen;

        /// <summary>
        /// Графика для этого канваса.
        /// </summary>
        private readonly Graphics _graphics;

        /// <summary>
        /// Изображение.
        /// </summary>
        private readonly PictureBox _imageBox;

        /// <summary>
        /// Начальная позиция признака.
        /// </summary>
        private Point _startPosition;

        /// <summary>
        /// Клавиша мыши нажата.
        /// </summary>
        private bool _isMouseButtonPressed;

        #endregion

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="CanvasPresenter"/>
        /// </summary>
        /// <param name="canvasView">Отображение.</param>
        /// <param name="imageBox">Изображение.</param>
        public CanvasPresenter(UserControl canvasView, PictureBox imageBox)
        {
            _pen = new Pen(Color.Black, LineWidth)
            {
                DashStyle = DashStyle.Dash
            };

            _imageBox = imageBox;
            _imageBox.SizeMode = PictureBoxSizeMode.Zoom;
            _imageBox.SizeMode = PictureBoxSizeMode.AutoSize;
            _imageBox.MouseDown += StartPointEvent;
            _imageBox.MouseUp += FinishPointEvent;
            _imageBox.MouseMove += DrawRectangle;
            _graphics = _imageBox.CreateGraphics();

            canvasView.Controls.Add(_imageBox);
            SignPosition = Rectangle.Empty;
            _isMouseButtonPressed = false;
        }

        /// <summary>
        /// Получает положение признака.
        /// </summary>
        public Rectangle SignPosition { get; private set; }

        /// <summary>
        /// Обработка событий при задании начальной точки признака.
        /// </summary>
        /// <param name="sender">Отправитель.</param>
        /// <param name="args">Аргументы события.</param>
        public void StartPointEvent(object sender, MouseEventArgs args)
        {
            _startPosition = args.Location;
            _isMouseButtonPressed = true;
        }

        /// <summary>
        /// Обработка событий при задании конечной точки признака.
        /// </summary>
        /// <param name="sender">Отправитель.</param>
        /// <param name="args">Аргументы события.</param>
        public void FinishPointEvent(object sender, MouseEventArgs args)
        {
            var startX = args.X - _startPosition.X > 0 ? _startPosition.X : args.X;
            var startY = args.Y - _startPosition.Y > 0 ? _startPosition.Y : args.Y;
            SignPosition = new Rectangle(startX, startY, Math.Abs(args.X - _startPosition.X), Math.Abs(args.Y - _startPosition.Y));
            DrawRectangle(sender, args);
            _isMouseButtonPressed = false;
        }

        /// <summary>
        /// Прорисовка прямоугольной области.
        /// </summary>
        /// <param name="sender">Отправитель.</param>
        /// <param name="args">Аргументы события.</param>
        public void DrawRectangle(object sender, MouseEventArgs args)
        {
            if (!_isMouseButtonPressed)
                return;

            var startX = args.X - _startPosition.X > 0 ? _startPosition.X : args.X;
            var startY = args.Y - _startPosition.Y > 0 ? _startPosition.Y : args.Y;
            _imageBox.Invalidate();
            _graphics.DrawRectangle(_pen, startX, startY, Math.Abs(args.X - _startPosition.X), Math.Abs(args.Y - _startPosition.Y));
        }
    }
}