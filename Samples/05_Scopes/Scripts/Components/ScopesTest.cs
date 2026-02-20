using UnityEngine;

namespace DependencyInjection.Samples._05_Scopes
{
    public class ScopesTest : MonoBehaviour
    {
        [Inject] private ILogger _logger;

        private void Start()
        {
            TestScopeIsolation();
            TestAutoDispose();
        }

        private void TestScopeIsolation()
        {
            _logger.Log("\n=== Testing Scope Isolation ===");

            using (var scope = DIContext.Container.CreateScope())
            {
                var trans = scope.Resolve<ITransactionService>();
                trans.BeginTransaction();
                trans.Commit();
            }

            _logger.Log("✓ Scope disposed\n");
        }

        private void TestAutoDispose()
        {
            _logger.Log("=== Testing Auto Dispose ===");

            using (var scope = DIContext.Container.CreateScope())
            {
                var trans = scope.Resolve<ITransactionService>();
                trans.BeginTransaction();
                // Pas de commit – va rollback automatiquement
            }

            _logger.Log("✓ Auto rollback on dispose\n");
        }
    }
}
