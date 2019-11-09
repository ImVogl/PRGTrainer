namespace PRGTrainer.Core.TelegramHandler.MessageProcessing
{
    using System.Collections.Generic;
    using StatesController;
    using Telegram.Bot.Args;
    using Telegram.Bot.Types.Enums;

    /// <summary>
    /// Обработчик сообщений при работе с книгой.
    /// </summary>
    public class ReferenceBookProcessing : IMessageProcessing
    {
        #region Private fields

        /// <summary>
        /// Контроллер состояния пользователя.
        /// </summary>
        private readonly IReferenceBookStateController _testStateController;

        /// <summary>
        /// Текущая схема уровней справочника.
        /// </summary>
        private readonly Dictionary<int, List<int>> _refDeepLevels;

        #endregion

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="ReferenceBookProcessing"/>
        /// </summary>
        /// <param name="testStateController">Контроллер состояния пользователя.</param>
        public ReferenceBookProcessing(IReferenceBookStateController testStateController)
        {
            _refDeepLevels = new Dictionary<int, List<int>>();
            _testStateController = testStateController;
        }

        /// <inheritdoc />
        public void OnMessage(object sender, MessageEventArgs eventArgs)
        {
            var message = eventArgs.Message;
            if (message == null || message.Type != MessageType.Text)
                return;

            if (!_testStateController.IsUserUsingRefBook(message.From.Id))
                return;

            if (message.Text == _testStateController.FinishCommand)
            {
                _refDeepLevels.Remove(message.From.Id);
                _testStateController.ResetState(message.From.Id);
                return;
            }

            ProcessMessage(message.From.Id, message.From.Username, message.Text);
        }

        #region Private region

        /// <summary>
        /// Обработка команд.
        /// </summary>
        /// <param name="id">Идентификатор пользователя.</param>
        /// <param name="user">Имя пользователя.</param>
        /// <param name="command">Команда.</param>
        private void ProcessMessage(int id, string user, string command)
        {
            //
        }

        #endregion
    }
}