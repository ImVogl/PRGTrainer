namespace SignExtractor.MainHost
{
    using System.Collections.Generic;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Windows.Forms;
    using Canvas;

    /// <summary>
    /// Презентер основного отображения.
    /// </summary>
    public class MainHostPresenter : IMainHostPresenter
    {
        #region Private fields

        /// <summary>
        /// Коллекция поисковых паттернов изображений.
        /// </summary>
        private static readonly List<string> SuitablePatterns = new List<string>{ @"*.png", @"*.jpg", @"*.jpeg" };

        /// <summary>
        /// Отображение.
        /// </summary>
        private readonly MainHostView _view;

        /// <summary>
        /// Коллекция контролов с канвасами.
        /// </summary>
        private readonly List<CanvasView> _canvasControls;

        /// <summary>
        /// Коллекция путей до файлов изображений.
        /// </summary>
        private readonly List<string> _imagePaths;

        /// <summary>
        /// Коллекция областей, где расположены признаки.
        /// </summary>
        private readonly Dictionary<string, Rectangle> _signAreas;

        /// <summary>
        /// Текущий контрол с канвас.
        /// </summary>
        private int _currentCanvas;

        #endregion

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="MainHostPresenter"/>
        /// </summary>
        /// <param name="view">Отображение.</param>
        public MainHostPresenter(MainHostView view)
        {
            _view = view;
            _canvasControls = new List<CanvasView>();
            _imagePaths = new List<string>();
            _signAreas = new Dictionary<string, Rectangle>();
            _view.scbScale.Scroll += ChangeImageScale;
            _currentCanvas = -1;
        }

        /// <inheritdoc />
        public bool IsImagesListEmpty
        {
            get { return !_canvasControls.Any(); }
        }

        /// <inheritdoc />
        public void SetImages()
        {
            _imagePaths.Clear();
            _signAreas.Clear();
            using (var selectFolderWindow = new FolderBrowserDialog())
            {
                selectFolderWindow.ShowNewFolderButton = false;
                var result = selectFolderWindow.ShowDialog();
                if (result != DialogResult.OK || string.IsNullOrWhiteSpace(selectFolderWindow.SelectedPath))
                    return;

                foreach (var extension in SuitablePatterns)
                    _imagePaths.AddRange(Directory.GetFiles(selectFolderWindow.SelectedPath, extension, SearchOption.TopDirectoryOnly));
            }

            if (_imagePaths.Any())
                UpdateCanvas();
        }

        /// <inheritdoc />
        public bool SetPreviousCanvas()
        {
            if (_currentCanvas <= 0)
                return false;

            _canvasControls[_currentCanvas].Hide();
            _currentCanvas--;
            _canvasControls[_currentCanvas].Show();
            _view.PathToImageLabel.Text = Path.GetFileName(_imagePaths[_currentCanvas]);
            return _currentCanvas > 0;
        }

        /// <inheritdoc />
        public bool SetNextCanvas()
        {
            if (_currentCanvas == _canvasControls.Count - 1)
                return false;

            _canvasControls[_currentCanvas].Hide();
            _currentCanvas++;
            _canvasControls[_currentCanvas].Show();
            _view.PathToImageLabel.Text = Path.GetFileName(_imagePaths[_currentCanvas]);
            return _currentCanvas < _canvasControls.Count - 1;
        }

        /// <inheritdoc />
        public bool RemoveCanvas()
        {
            if (_currentCanvas < _canvasControls.Count - 1)
            {
                _canvasControls[_currentCanvas].Hide();
                _canvasControls[_currentCanvas].Dispose();
                _view.Controls.Remove(_canvasControls[_currentCanvas]);
                _canvasControls.RemoveAt(_currentCanvas);
                File.Delete(_imagePaths[_currentCanvas]);
                _canvasControls[_currentCanvas].Show();
                _imagePaths.RemoveAt(_currentCanvas);
            }
            else
            {
                _canvasControls[_currentCanvas].Hide();
                _canvasControls[_currentCanvas].Dispose();
                _view.Controls.Remove(_canvasControls[_currentCanvas]);
                _canvasControls.RemoveAt(_currentCanvas);
                File.Delete(_imagePaths[_currentCanvas]);
                _imagePaths.RemoveAt(_currentCanvas);
                _canvasControls[--_currentCanvas].Show();
            }

            return _currentCanvas < _canvasControls.Count - 1;
        }

        /// <inheritdoc />
        public void SaveSignPositions()
        {
            var areas = new List<string>();
            foreach (var path in _signAreas.Keys)
                if (_signAreas[path] != Rectangle.Empty)
                    areas.Add(
                        $"{Path.GetFileName(path)}\t{_signAreas[path].X}\t{_signAreas[path].Y}\t{_signAreas[path].X + _signAreas[path].Width}\t{_signAreas[path].Y + _signAreas[path].Height}");
            
            using (var saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.FileName = @"ObjectPositions.txt";
                var result = saveFileDialog.ShowDialog();
                if (result != DialogResult.OK || string.IsNullOrWhiteSpace(saveFileDialog.FileName))
                    return;

                File.WriteAllLines(saveFileDialog.FileName, areas, Encoding.Default);
            }
        }

        #region Private methods

        /// <summary>
        /// Изменение масштаба изображения для текущего канваса.
        /// </summary>
        /// <param name="sender">Отправитель.</param>
        /// <param name="args">Аргументы события.</param>
        private void ChangeImageScale(object sender, ScrollEventArgs args)
        {
            var scrollBar = sender as HScrollBar;
            if (scrollBar == null)
                return;

            var progress = 1 - (float)scrollBar.Value / (scrollBar.Maximum - scrollBar.Minimum);
            _canvasControls[_currentCanvas].SetImageScale(progress);
        }

        /// <summary>
        /// обновление всех областей с прорисованными изображениями.
        /// </summary>
        private void UpdateCanvas()
        {
            foreach (var control in _canvasControls)
                _view.Controls.Remove(control);

            _canvasControls.Clear();
            _currentCanvas = 0;
            foreach (var path in _imagePaths)
            {
                var control = new CanvasView(_view, new PictureBox { Image = Image.FromFile(path), Cursor = Cursors.Cross });
                control.PropertyChanged += (sender, args) =>
                {
                    if (args.PropertyName == @"Area")
                        SetSignArea(control.Area);
                };

                _canvasControls.Add(control);
                _view.Controls.Add(control);
            }

            _canvasControls[_currentCanvas].Show();
            _canvasControls[_currentCanvas].Focus();
            _view.PathToImageLabel.Text = Path.GetFileName(_imagePaths[_currentCanvas]);
        }

        /// <summary>
        /// Задание области с признаком.
        /// </summary>
        /// <param name="area">Область с признаком.</param>
        private void SetSignArea(Rectangle area)
        {
            if (area == Rectangle.Empty)
                return;

            _signAreas[_imagePaths[_currentCanvas]] = area;
        }

        #endregion
    }
}