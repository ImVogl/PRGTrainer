namespace SignExtractor.MainHost
{
    using System;
    using System.Drawing;
    using System.Windows.Forms;

    /// <summary>
    /// Основное отображение.
    /// </summary>
    public partial class MainHostView : Form
    {
        #region Private fields

        /// <summary>
        /// Презентер для данного отображения.
        /// </summary>
        private IMainHostPresenter _presenter;

        #endregion
        /// <summary>
        /// Инициализирует новый экземпляр <see cref="MainHostView"/>
        /// </summary>
        public MainHostView()
        {
            InitializeComponent();
            CustomInitialize();
        }

        #region Private methods

        /// <summary>
        /// Инициализация произвольных полей.
        /// </summary>
        private void CustomInitialize()
        {
            _presenter = new MainHostPresenter(this);
            btnSelectWorkDir.Click += (sender, args) =>
            {
                _presenter.SetImages();
                ChangeStateOfControls();
                btnNextImage.Enabled = true;
            };

            btnNextImage.Click += (sender, args) =>
            {
                var button = sender as Button;
                if (button == null)
                    return;

                button.Enabled = _presenter.SetNextCanvas();
                btnPreviousImage.Enabled = true;
            };

            btnPreviousImage.Click += (sender, args) => 
            {
                var button = sender as Button;
                if (button == null)
                    return;

                button.Enabled = _presenter.SetPreviousCanvas();
                btnNextImage.Enabled = true;
            };

            btnRemoveImage.Click += (sender, args) =>
            {
                btnNextImage.Enabled = _presenter.RemoveCanvas();
                ChangeStateOfControls();
            };

            btnSaveResult.Click += (sender, args) => _presenter.SaveSignPositions();
            SizeChanged += ResizeWindow;
            btnPreviousImage.Enabled = false;
            btnNextImage.Enabled = false;
            ChangeStateOfControls();
        }

        /// <summary>
        /// Изменение состояния активности контроллов.
        /// </summary>
        private void ChangeStateOfControls()
        {
            scbScale.Enabled = !_presenter.IsImagesListEmpty;
            btnSaveResult.Enabled = !_presenter.IsImagesListEmpty;
            btnRemoveImage.Enabled = !_presenter.IsImagesListEmpty;
        }

        /// <summary>
        /// Изменение размера окна.
        /// </summary>
        /// <param name="sender">Отправитель.</param>
        /// <param name="args">Аргументы события.</param>
        private void ResizeWindow(object sender, EventArgs args)
        {
            var form = sender as MainHostView;
            if (form == null)
                return;

            btnPreviousImage.Location = new Point(10, form.Height - 90);
            btnNextImage.Location = new Point(60, form.Height - 90);
            PathToImageLabel.Location = new Point(120, form.Height - 90);
            btnSelectWorkDir.Location = new Point(form.Width - 120, 10);
            btnRemoveImage.Location = new Point(form.Width - 120, 35);
            scbScale.Location = new Point(form.Width - 210, form.Height - 90);
            lbScale.Location = new Point(form.Width - 210, form.Height - 65);
            btnSaveResult.Location = new Point(form.Width - 120, 60);
        }

        #endregion
    }
}
