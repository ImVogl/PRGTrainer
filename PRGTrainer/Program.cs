namespace PRGTrainer
{
    using Autofac;
    using Core;
    using Core.TelegramHandler;
    using Topshelf;

    class Program
    {
        static void Main(string[] args)
        {
            var builder = new ContainerBuilder();
            builder.RegisterModule<TrainerModule>();
            var container = builder.Build();
            var handler = container.Resolve<ITelegramHandler>();

            HostFactory.Run(
                host =>
                {
                    host.RunAsNetworkService().StartAutomatically();

                    host.Service<ITelegramHandler>(
                        configurator =>
                        {
                            configurator.ConstructUsing(() => handler);
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
