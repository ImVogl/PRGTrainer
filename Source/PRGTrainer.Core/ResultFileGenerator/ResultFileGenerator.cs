﻿namespace PRGTrainer.Core.ResultFileGenerator
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using Model.Result;
    using ScottPlot;

    /// <summary>
    /// Генератор результирующих файлов.
    /// </summary>
    public class ResultFileGenerator : IResultFileGenerator
    {
        #region Private fields

        /// <summary>
        /// Ширина графика.
        /// </summary>
        private const int Width = 1920;

        /// <summary>
        /// Высота графика.
        /// </summary>
        private const int High = 1440;

        /// <summary>
        /// Значение минимального значения на оси Х.
        /// </summary>
        private const int MinPointX = 0;

        /// <summary>
        /// Значение максимального значения на оси Х.
        /// </summary>
        private const double MaxPointX = 100;

        /// <summary>
        /// Значение минимального значения на оси Y.
        /// </summary>
        private const int MinPointY = 0;

        /// <summary>
        /// Значение максимального значения на оси Y.
        /// </summary>
        private const double MaxPointY = 100;

        /// <summary>
        /// Размер шрифта.
        /// </summary>
        private const float FontSize = 10;

        /// <summary>
        /// Путь до рабочей директории.
        /// </summary>
        private readonly string _workFolderPath;

        /// <summary>
        /// Генератор случайного имени.
        /// </summary>
        private readonly Random _random;

        #endregion

        /// <summary>
        /// Создает экземпляр <see cref="ResultFileGenerator"/>
        /// </summary>
        public ResultFileGenerator()
        {
            _workFolderPath = Path.Combine(Assembly.GetAssembly(typeof(ResultFileGenerator)).Location, @"..\Results");
            Directory.CreateDirectory(_workFolderPath);
            _random = new Random();
        }
        
        /// <inheritdoc />
        public string GenerateAsText(IEnumerable<QuestionResult> questionResults)
        {
            var path = Path.Combine(_workFolderPath, GenerateName() + @".txt");
            if (File.Exists(path))
                File.Delete(path);

            if(questionResults.Any())
                File.WriteAllLines(path, questionResults.Select(result => $"{result.Question}\t{result.Quota:P3};"));
            else
                File.WriteAllText(path, @"(Нет результатов, отвечающих установленным критериям)");  

            return path;
        }

        /// <inheritdoc />
        public string GenerateAsText(IEnumerable<UserResult> userResults)
        {
            var path = Path.Combine(_workFolderPath, GenerateName() + @".txt");
            if(File.Exists(path))
                File.Delete(path);

            if (!userResults.Any())
            {
                File.WriteAllText(path, @"(Нет результатов, отвечающих установленным критериям)");
                return path;
            }

            File.WriteAllText(path, @"");
            foreach (var result in userResults)
            {
                File.AppendAllText(path, result.User + Environment.NewLine);
                File.AppendAllLines(path, result.Result.Select(c => $"{c.Key}\t{c.Value};"));
                File.AppendAllText(path, Environment.NewLine);
            }

            return path;
        }

        /// <inheritdoc />
        public string GenerateAsImage(IEnumerable<QuestionResult> questionResults)
        {
            var path = Path.Combine(_workFolderPath, GenerateName() + @".png");
            var plot = new Plot(Width, High);
            var startPoint = (double)MinPointX;
            foreach (var questionResult in questionResults)
            {
                var endPoint = startPoint + questionResult.Quota * (MaxPointX - MinPointX);
                plot.PlotVSpan(startPoint, endPoint, label: SplitTooLongLabel(questionResult.Question));
                startPoint = endPoint;
            }

            plot.XLabel(@"Доля неверных ответов.", fontSize: 4 * FontSize);
            plot.Axis(MinPointX, MaxPointX, MinPointY, MaxPointY);
            plot.Legend(fontSize: FontSize);
            plot.SaveFig(path);

            return path;
        }

        /// <inheritdoc />
        public string GenerateAsImage(IEnumerable<UserResult> userResults)
        {
            var path = Path.Combine(_workFolderPath, GenerateName() + @".png");
            var plot = new Plot(Width, High);
            if (userResults.Any())
            {
                var minimum = userResults.Min(result => result.Result.Keys.Min());
                var maximum = userResults.Max(result => result.Result.Keys.Max());
                foreach (var result in userResults)
                {
                    plot.PlotScatter(
                        ConvertDateToDouble(result.Result.Keys, minimum, maximum).ToArray(),
                        result.Result.Values.Select(item => (double) item).ToArray(),
                        label: result.User,
                        markerSize: 10);

                    SetLabels(
                        plot,
                        result.Result.Keys,
                        result.Result.Values.Select(item => (double) item), minimum,
                        maximum);
                }

                plot.XLabel(
                    (maximum - minimum).TotalHours <= 24 
                        ? @"Время прохождения теста." 
                        : @"Дата прохождения теста.",
                    fontSize: 4 * FontSize);
            }

            plot.YLabel(@"Доля верных ответов.", fontSize: 4 * FontSize);
            plot.Axis(MinPointX, MaxPointX, MinPointY, MaxPointY);
            plot.Legend(fontSize: 2*FontSize);
            plot.SaveFig(path);

            return path;
        }

        /// <summary>
        /// Преобразование даты в число.
        /// </summary>
        /// <param name="dates">Коллекция дат.</param>
        /// <param name="min">Наиболее ранняя дата прохождения теста.</param>
        /// <param name="max">Последняя дата прохождения теста.</param>
        /// <returns>Коллекция значений на оси Х.</returns>
        private static IEnumerable<double> ConvertDateToDouble(IEnumerable<DateTime> dates, DateTime min, DateTime max)
        {
            var span = (max - min).TotalDays;
            foreach (var date in dates)
                yield return (MaxPointX - MinPointX)*(date - min).TotalDays / span;
        }

        /// <summary>
        /// Генерация случайного имени файла.
        /// </summary>
        /// <returns>Имя файла.</returns>
        private string GenerateName()
        {
            return _random.Next(10000, int.MaxValue).ToString();
        }

        /// <summary>
        /// Задает метки с датой на графике
        /// </summary>
        /// <param name="plot">График.</param>
        /// <param name="datas">Даты.</param>
        /// <param name="dataY">Значения положений меток.</param>
        /// <param name="min">Наиболее ранняя дата прохождения теста.</param>
        /// <param name="max">Последняя дата прохождения теста.</param>
        private static void SetLabels(Plot plot, IEnumerable<DateTime> datas, IEnumerable<double> dataY, DateTime min, DateTime max)
        {
            const int division = 30;
            const double offsetX = 7.5; 
            var datasLoc = datas.ToList();
            var dateFormat = (max - min).TotalHours <= 24 ? @"t" : @"d";
            var dataX = ConvertDateToDouble(datasLoc, min, max).ToList();
            for (var i = 0; i < datasLoc.Count; i++)
            {
                var y = dataY.ToList()[i];
                    y = y < (MaxPointY - MinPointY) / 2
                    ? y + (MaxPointY - MinPointY) / division
                    : y - (MaxPointY - MinPointY) / division;

                var x = dataX[i] > MaxPointX - offsetX ? MaxPointX - offsetX : dataX[i];
                plot.PlotText(datasLoc[i].ToString(dateFormat), x, y, Color.Black, fontSize: 2*FontSize);
            }
        }

        /// <summary>
        /// Добавление переноса строки в слишком длинные сообщения.
        /// </summary>
        /// <param name="message">Сообщение.</param>
        /// <returns>Разделенные на несколько строк сообщение.</returns>
        private static string SplitTooLongLabel(string message)
        {
            const int maxLineLength = 200;
            if (message.Length <= maxLineLength)
                return Environment.NewLine + message;

            var lines = new List<string> { string.Empty };
            var index = 0;
            var words = message.Split(' ').Where(word => !string.IsNullOrWhiteSpace(word));
            foreach (var word in words)
            {
                if (lines[index].Length <= maxLineLength)
                    lines[index] += @" " + word;
                else
                {
                    lines.Add(word);
                    index++;
                }
            }

            return Environment.NewLine + string.Join(Environment.NewLine, lines);
        }
    }
}