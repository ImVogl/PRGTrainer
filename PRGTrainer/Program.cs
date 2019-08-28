namespace PRGTrainer
{
    using Core.TasksReaders;
    using Core.TasksStorage;
    using Core.TelegramHandler;
    using Topshelf;

    class Program
    {
        static void Main(string[] args)
        {
            var taskStorage = new TasksStorage(new PRGTasksReader());

            HostFactory.Run(
                host =>
                {
                    host.RunAsNetworkService().StartAutomatically();

                    host.Service<ITelegramHandler>(
                        configurator =>
                        {
                            configurator.ConstructUsing(() => new TelegramHandler(taskStorage));
                            configurator.WhenStarted(m => m.InitialiseSession());
                            configurator.WhenStopped(m => m.Dispose());
                            configurator.WhenPaused(m => { });
                            configurator.WhenContinued(m => { });
                        });

                    host.SetDescription("Telegram бот для обучения членов комиссии с правом решающего голоса.");
                    host.SetDisplayName("Telegram бот обучения ПРГ.");
                    host.SetServiceName("PRGTrainer");
                });
        }
    }
}
