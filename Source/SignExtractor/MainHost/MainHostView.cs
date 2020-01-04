namespace SignExtractor.MainHost
{
    using System;
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
        private MainHostPresenter _presenter;

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
            btnSelectWorkDir.Click += (sender, args) => _presenter.SetImages();
            btnNextImage.Click += (sender, args) =>
            {
                var button = sender as Button;
                if (button == null)
                    return;

                button.Enabled = _presenter.SetNextCanvas();
            };

            btnPreviousImage.Click += (sender, args) => 
            {
                var button = sender as Button;
                if (button == null)
                    return;

                button.Enabled = _presenter.SetPreviousCanvas();
            };

            btnRemoveImage.Click += (sender, args) =>
            {
                btnNextImage.Enabled = _presenter.RemoveCanvas();
            };

            SizeChanged += ResizeWindow;
            btnPreviousImage.Enabled = false;
            btnNextImage.Enabled = false;
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

            btnPreviousImage.Location = new System.Drawing.Point(10, form.Height - 90);
            btnNextImage.Location = new System.Drawing.Point(60, form.Height - 90);
            PathToImageLabel.Location = new System.Drawing.Point(120, form.Height - 90);
            btnSelectWorkDir.Location = new System.Drawing.Point(form.Width - 120, 10);
            btnRemoveImage.Location = new System.Drawing.Point(form.Width - 120, 35);
        }

        #endregion
    }
}
