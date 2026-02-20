using UnityEngine;

namespace DependencyInjection.Examples._01_BasicInjection
{
    public class BasicInjectionContext : DIContext
    {
        protected override void ConfigureServices(DIContainer container)
        {
            container.Register<ILogger, ConsoleLogger>(Lifetime.Singleton);
            Debug.Log("[BasicInjection] Services configured");
        }
    }
}
