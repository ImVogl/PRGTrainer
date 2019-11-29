namespace PRGTrainer.Core.ResultFileGenerator
{
    using System.Collections.Generic;
    using Model.Result;

    /// <summary>
    /// Генератор результирующих файлов.
    /// </summary>
    public class ResultFileGenerator : IResultFileGenerator
    {
        /// <inheritdoc />
        public string GenerateAsText(IEnumerable<QuestionResult> questionResults)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc />
        public string GenerateAsText(IEnumerable<UserResult> userResults)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc />
        public string GenerateAsImage(IEnumerable<QuestionResult> questionResults)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc />
        public string GenerateAsImage(IEnumerable<UserResult> userResults)
        {
            throw new System.NotImplementedException();
        }
    }
}