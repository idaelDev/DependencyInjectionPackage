using UnityEngine;

namespace IdaelDev.DependencyInjection.Samples._02_ConstructorInjection
{
    public class ConstructorInjectionContext : DIContext
    {
        protected override void ConfigureServices(DIContainer container)
        {
            container.Register<ILogger, ConsoleLogger>(Lifetime.Singleton);
            container.Register<IAudioService, AudioService>(Lifetime.Singleton);
            container.Register<IGameManager, GameManager>(Lifetime.Singleton);

            Debug.Log("[ConstructorInjection] Services configured");
        }
    }
}
