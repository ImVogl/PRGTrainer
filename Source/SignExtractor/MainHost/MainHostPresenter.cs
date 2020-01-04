namespace SignExtractor.MainHost
{
    using System.Collections.Generic;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using System.Windows.Forms;
    using Canvas;

    /// <summary>
    /// Презентер основного отображения.
    /// </summary>
    public class MainHostPresenter
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
        private readonly List<Control> _canvasControls;

        /// <summary>
        /// Коллекция путей до файлов изображений.
        /// </summary>
        private readonly List<string> _imagePaths;

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
            _canvasControls = new List<Control>();
            _imagePaths = new List<string>();
            _currentCanvas = -1;
        }

        /// <summary>
        /// Действие при нажатии на 
        /// </summary>
        public void SetImages()
        {
            _imagePaths.Clear();
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
                UpdateCanvas(); ;
        }

        /// <summary>
        /// Задание предыдущего отображения.
        /// </summary>
        /// <returns>Значение, показывающее, что контрол не является первым.</returns>
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

        /// <summary>
        /// Задание следующего отображения.
        /// </summary>
        /// <returns>Значение, показывающее, что контрол не является последним.</returns>
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

        /// <summary>
        /// Удаление текущего изображения.
        /// </summary>
        /// <returns>Значение, показывающее, что контрол не является последним.</returns>
        public bool RemoveCanvas()
        {
            if (_currentCanvas == _canvasControls.Count - 1)
            {
                _canvasControls[_currentCanvas].Hide();
                _view.Controls.Remove(_canvasControls[_currentCanvas]);
                _canvasControls.RemoveAt(_currentCanvas);
                _canvasControls[_currentCanvas].Show();
                _imagePaths.RemoveAt(_currentCanvas);
            }
            else
            {
                _canvasControls[_currentCanvas].Hide();
                _view.Controls.Remove(_canvasControls[_currentCanvas]);
                _canvasControls.RemoveAt(_currentCanvas);
                _imagePaths.RemoveAt(_currentCanvas);
                _canvasControls[_currentCanvas--].Show();
            }

            return _currentCanvas < _canvasControls.Count - 1;
        }

        #region Private methods

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
                var control = new CanvasView(_view, new PictureBox { Image = Image.FromFile(path) });
                _canvasControls.Add(control);
                _view.Controls.Add(control);
            }

            _canvasControls[_currentCanvas].Show();
            _view.PathToImageLabel.Text = Path.GetFileName(_imagePaths[_currentCanvas]);
        }

        #endregion
    }
}