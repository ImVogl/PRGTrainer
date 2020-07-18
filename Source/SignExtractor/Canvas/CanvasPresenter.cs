﻿namespace SignExtractor.Canvas
{
    using System;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Windows.Forms;

    /// <summary>
    /// Презентер области прорисовки изображения.
    /// </summary>
    public class CanvasPresenter : ICanvasPresenter
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
        /// Отображение.
        /// </summary>
        private readonly CanvasView _view;

        /// <summary>
        /// Начальная позиция признака.
        /// </summary>
        private Point _startPosition;

        /// <summary>
        /// Клавиша мыши нажата.
        /// </summary>
        private bool _isMouseButtonPressed;

        /// <summary>
        /// Коэффициент сжатия изображения.
        /// </summary>
        private float _coefficient;

        #endregion

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="CanvasPresenter"/>
        /// </summary>
        /// <param name="canvasView">Отображение.</param>
        /// <param name="imageBox">Изображение.</param>
        public CanvasPresenter(CanvasView canvasView, PictureBox imageBox)
        {
            _pen = new Pen(Color.Black, LineWidth)
            {
                DashStyle = DashStyle.Dash
            };

            _coefficient = 1;
            _imageBox = imageBox;
            _imageBox.Size = imageBox.Image.Size;
            _imageBox.SizeMode = PictureBoxSizeMode.Zoom;
            _imageBox.MouseDown += StartPointEvent;
            _imageBox.MouseUp += FinishPointEvent;
            _imageBox.MouseMove += DrawRectangle;
            _graphics = _imageBox.CreateGraphics();

            canvasView.Controls.Add(_imageBox);
            _view = canvasView;
            SignPosition = Rectangle.Empty;
            _isMouseButtonPressed = false;
        }

        /// <inheritdoc />
        public Rectangle SignPosition { get; private set; }

        /// <inheritdoc />
        public void SetImageScale(float progress)
        {
            var maxCoefficient = Math.Max((float)_imageBox.Image.Width / _view.Width, (float)_imageBox.Image.Height / _view.Height);
            _coefficient = 1 + progress*(maxCoefficient - 1);
            _imageBox.Size = new Size((int)(_imageBox.Image.Size.Width / _coefficient), (int)(_imageBox.Image.Size.Height / _coefficient));
        }

        /// <inheritdoc />
        public void Dispose()
        {
            _imageBox.Image.Dispose();
            _imageBox.Image = null;
            _imageBox.Dispose();
        }

        #region Private methods

        /// <summary>
        /// Обработка событий при задании начальной точки признака.
        /// </summary>
        /// <param name="sender">Отправитель.</param>
        /// <param name="args">Аргументы события.</param>
        private void StartPointEvent(object sender, MouseEventArgs args)
        {
            _startPosition = args.Location;
            _isMouseButtonPressed = true;
        }

        /// <summary>
        /// Обработка событий при задании конечной точки признака.
        /// </summary>
        /// <param name="sender">Отправитель.</param>
        /// <param name="args">Аргументы события.</param>
        private void FinishPointEvent(object sender, MouseEventArgs args)
        {
            var startX = (int)(_coefficient * (args.X - _startPosition.X > 0 ? _startPosition.X : args.X));
            var startY = (int)(_coefficient * (args.Y - _startPosition.Y > 0 ? _startPosition.Y : args.Y));
            var width = (int) (_coefficient * Math.Abs(args.X - _startPosition.X));
            var height = (int)(_coefficient * Math.Abs(args.Y - _startPosition.Y));

            SignPosition = new Rectangle(startX, startY, width, height);
            DrawRectangle(sender, args);
            _isMouseButtonPressed = false;
        }

        /// <summary>
        /// Прорисовка прямоугольной области.
        /// </summary>
        /// <param name="sender">Отправитель.</param>
        /// <param name="args">Аргументы события.</param>
        private void DrawRectangle(object sender, MouseEventArgs args)
        {
            if (!_isMouseButtonPressed)
                return;

            var startX = args.X - _startPosition.X > 0 ? _startPosition.X : args.X;
            var startY = args.Y - _startPosition.Y > 0 ? _startPosition.Y : args.Y;
            _imageBox.Invalidate();
            _graphics.DrawRectangle(_pen, startX, startY, Math.Abs(args.X - _startPosition.X), Math.Abs(args.Y - _startPosition.Y));
        }

        #endregion
    }
}