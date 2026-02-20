using UnityEngine;

namespace DependencyInjection.Examples._06_RuntimeInstantiation
{
    public class RuntimeInstantiationContext : DIContext
    {
        protected override void ConfigureServices(DIContainer container)
        {
            container.Register<ILogger, ConsoleLogger>(Lifetime.Singleton);
            Debug.Log("[RuntimeInstantiation] Services configured");
        }
    }
}
