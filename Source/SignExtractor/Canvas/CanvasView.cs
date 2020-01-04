namespace SignExtractor.Canvas
{
    using System.ComponentModel;
    using System.Drawing;
    using System.Runtime.CompilerServices;
    using System.Windows.Forms;
    using Annotations;

    /// <summary>
    /// Отображение области прорисовки изображения.
    /// </summary>
    public partial class CanvasView : UserControl, ICanvasView, INotifyPropertyChanged
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
        private ICanvasPresenter _canvasPresenter;

        /// <summary>
        /// Область с признаком.
        /// </summary>
        private Rectangle _area;

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

        /// <inheritdoc />
        public event PropertyChangedEventHandler PropertyChanged;

        /// <inheritdoc />
        public Rectangle Area
        {
            get { return _area; }
            private set
            {
                if (_area == value || value.Height*value.Width == 0)
                    return;

                _area = value;
                OnPropertyChanged(nameof(Area));
            }
        }

        /// <inheritdoc />
        public void SetImageScale(float progress)
        {
            _canvasPresenter.SetImageScale(progress);
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
            imageBox.MouseUp += (sender, args) =>
            {
                Area = _canvasPresenter.SignPosition;
            };

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

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
