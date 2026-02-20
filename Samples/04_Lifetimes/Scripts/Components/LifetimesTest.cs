using UnityEngine;

namespace DependencyInjection.Samples._04_Lifetimes
{
    public class LifetimesTest : MonoBehaviour
    {
        [Inject] private ILogger _logger;

        private void Start()
        {
            TestSingleton();
            TestTransient();
            TestScoped();
        }

        private void TestSingleton()
        {
            var s1 = DIContext.Container.Resolve<ISingletonService>();
            var s2 = DIContext.Container.Resolve<ISingletonService>();

            bool pass = s1.GetId() == s2.GetId();
            _logger.Log($"Singleton: {(pass ? "✓ PASS" : "✗ FAIL")} (IDs: {s1.GetId()}, {s2.GetId()})");
        }

        private void TestTransient()
        {
            var t1 = DIContext.Container.Resolve<ITransientService>();
            var t2 = DIContext.Container.Resolve<ITransientService>();

            bool pass = t1.GetId() != t2.GetId();
            _logger.Log($"Transient: {(pass ? "✓ PASS" : "✗ FAIL")} (IDs: {t1.GetId()}, {t2.GetId()})");
        }

        private void TestScoped()
        {
            using var scope = DIContext.Container.CreateScope();
            var sc1 = scope.Resolve<IScopedService>();
            var sc2 = scope.Resolve<IScopedService>();

            bool pass = sc1.GetId() == sc2.GetId();
            _logger.Log($"Scoped (same scope): {(pass ? "✓ PASS" : "✗ FAIL")} (IDs: {sc1.GetId()}, {sc2.GetId()})");

            using var scope2 = DIContext.Container.CreateScope();
            var sc3 = scope2.Resolve<IScopedService>();
            bool pass2 = sc1.GetId() != sc3.GetId();
            _logger.Log($"Scoped (Different Scope): {(pass2 ? "✓ PASS" : "✗ FAIL")} (IDs: {sc1.GetId()}, {sc3.GetId()})");
        }
    }
}
