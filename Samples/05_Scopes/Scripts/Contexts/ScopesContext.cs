using UnityEngine;

namespace DependencyInjection.Samples._05_Scopes
{
    public class ScopesContext : DIContext
    {
        protected override void ConfigureServices(DIContainer container)
        {
            container.Register<ILogger, ConsoleLogger>(Lifetime.Singleton);
            container.Register<ITransactionService, TransactionService>(Lifetime.Scoped);
            Debug.Log("[Scopes] Services configured");
        }
    }
}
