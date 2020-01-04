namespace SignExtractor.Canvas
{
    using System.Drawing;
    using System.Windows.Forms;

    /// <summary>
    /// Отображение области прорисовки изображения.
    /// </summary>
    public partial class CanvasView : UserControl
    {
        #region Private fields

        /// <summary>
        /// Смещение по ширине.
        /// </summary>
        private const int OffsetX = 150;

        /// <summary>
        /// Смещение по высоте.
        /// </summary>
        private const int OffsetY = 110;

        /// <summary>
        /// Минимальная высота области, где производится работа с изображениями.
        /// </summary>
        private const int MinHeight = 200;

        /// <summary>
        /// Минимальная ширина области, где производится работа с изображениями.
        /// </summary>
        private const int MinWidth = 250;

        /// <summary>
        /// Презентер.
        /// </summary>
        private CanvasPresenter _canvasPresenter;

        #endregion

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="CanvasView"/>
        /// </summary>
        /// <param name="imageBox">Изображение.</param>
        /// <param name="form">Родительская форма.</param>
        public CanvasView(Form form, PictureBox imageBox)
        {
            InitializeComponent();
            CustomInitialize(form, imageBox);
        }

        #region Private methods

        /// <summary>
        /// Инициализация производных параметров.
        /// </summary>
        /// <param name="imageBox">Изображение.</param>
        /// <param name="form">Родительская форма.</param>
        private void CustomInitialize(Form form, PictureBox imageBox)
        {
            Hide();
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            _canvasPresenter = new CanvasPresenter(this, imageBox);

            if (form == null)
                return;

            var width = form.Size.Width - OffsetX < MinWidth ? MinWidth : form.Size.Width - OffsetX;
            var height = form.Size.Height - OffsetY < MinHeight ? MinHeight : form.Size.Height - OffsetY;
            Size = new Size(width, height);
            Location = new Point(10, 10);
            form.SizeChanged += (sender, args) =>
            {
                width = form.Size.Width - OffsetX < MinWidth ? MinWidth : form.Size.Width - OffsetX;
                height = form.Size.Height - OffsetY < MinHeight ? MinHeight : form.Size.Height - OffsetY;
                Size = new Size(width, height);
            };
        }

        #endregion
    }
}
