using UnityEngine;

namespace DependencyInjection.Samples._03_AsyncInjection
{
    public class AsyncInjectionContext : DIContext
    {
        protected override void ConfigureServices(DIContainer container)
        {
            container.Register<ILogger, ConsoleLogger>(Lifetime.Singleton);
            container.Register<IDataService, DataService>(Lifetime.Singleton);
            Debug.Log("[AsyncInjection] Services configured");
        }
    }
}
