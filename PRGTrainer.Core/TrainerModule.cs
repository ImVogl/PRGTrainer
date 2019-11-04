namespace PRGTrainer.Core
{
    using Autofac;
    using StatisticsCollector;
    using TasksProcessing;
    using TasksReaders;
    using TasksStorage;
    using TelegramHandler;
    using TelegramHandler.StatesController;
    using TelegramHandler.TestAnswerProcessing;

    /// <summary>
    /// Регистрация классов.
    /// </summary>
    public class TrainerModule : Module
    {
        /// <inheritdoc />
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<FileTasksReader>()
                .As<ITasksReader>()
                .SingleInstance();

            builder.RegisterType<TasksStorage.TasksStorage>()
                .As<ITasksStorage>()
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

            builder.RegisterType<TelegramHandler.TelegramHandler>()
                .As<ITelegramHandler>()
                .SingleInstance();
        }
    }
}