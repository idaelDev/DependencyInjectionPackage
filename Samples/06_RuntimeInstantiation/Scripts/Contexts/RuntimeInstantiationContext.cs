using IdaelDev.DependencyInjection;
using UnityEngine;

namespace DependencyInjection.Samples._06_RuntimeInstantiation
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
