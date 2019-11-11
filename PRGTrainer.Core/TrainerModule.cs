namespace PRGTrainer.Core
{
    using System.Configuration;
    using Autofac;
    using ReferenceBookReaders;
    using ReferenceBookStorage;
    using StatisticsCollector;
    using TasksProcessing;
    using TasksReaders;
    using TasksStorage;
    using Telegram.Bot;
    using TelegramHandler;
    using TelegramHandler.MessageProcessing;
    using TelegramHandler.StatesController;

    /// <summary>
    /// Регистрация классов.
    /// </summary>
    public class TrainerModule : Module
    {
        /// <inheritdoc />
        protected override void Load(ContainerBuilder builder)
        {
            var token = ConfigurationManager.AppSettings[@"telegramToken"];
            builder.Register(c => new TelegramBotClient(token))
                .As<ITelegramBotClient>()
                .SingleInstance();

            builder.RegisterType<FileTasksReader>()
                .As<ITasksReader>()
                .SingleInstance();

            builder.RegisterType<FileReferenceBookReader>()
                .As<IReferenceBookReader>()
                .SingleInstance();

            builder.RegisterType<TasksStorage.TasksStorage>()
                .As<ITasksStorage>()
                .SingleInstance();

            builder.RegisterType<ReferenceBookStorage.ReferenceBookStorage>()
                .As<IReferenceBookStorage>()
                .SingleInstance();

            builder.RegisterType<StatisticsCollector.StatisticsCollector>()
                .As<IStatisticsCollector>()
                .SingleInstance();

            builder.RegisterType<TasksProcessing.TasksProcessing>()
                .As<ITasksProcessing>()
                .SingleInstance();

            builder.RegisterType<StatesController>()
                .As<ITestStateController>()
                .As<IReferenceBookStateController>()
                .As<IMessageProcessing>()
                .SingleInstance();

            builder.RegisterType<TestAnswerProcessing>()
                .As<IMessageProcessing>()
                .SingleInstance();

            builder.RegisterType<ReferenceBookProcessing>()
                .As<IMessageProcessing>()
                .SingleInstance();

            builder.RegisterType<TelegramHandler.TelegramHandler>()
                .As<ITelegramHandler>()
                .SingleInstance();
                
        }
    }
}