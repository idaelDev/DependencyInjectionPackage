using UnityEngine;
using ILogger = DependencyInjection.Samples._04_Lifetimes.ILogger;

namespace DependencyInjection.Samples._04_Lifetimes
{
    public class LifetimesContext : DIContext
    {
        protected override void ConfigureServices(DIContainer container)
        {
            container.Register<ILogger, ConsoleLogger>(Lifetime.Singleton);
            container.Register<ISingletonService, SingletonService>(Lifetime.Singleton);
            container.Register<ITransientService, TransientService>(Lifetime.Transient);
            container.Register<IScopedService, ScopedService>(Lifetime.Scoped);
            Debug.Log("[Lifetimes] Services configured");
        }
    }
}
