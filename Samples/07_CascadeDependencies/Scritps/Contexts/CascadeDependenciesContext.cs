using IdaelDev.DependencyInjection;
using UnityEngine;

namespace DependencyInjection.Samples._07_CascadeDependencies
{
    public class CascadeDependenciesContext : DIContext
    {
        protected override void ConfigureServices(DIContainer container)
        {
            container.Register<ILogger, ConsoleLogger>(Lifetime.Singleton);
            container.Register<IServiceC, ServiceC>(Lifetime.Singleton);
            container.Register<IServiceB, ServiceB>(Lifetime.Singleton);
            container.Register<IServiceA, ServiceA>(Lifetime.Singleton);
            Debug.Log("[CascadeDependencies] Services configured");
        }
    }
}
