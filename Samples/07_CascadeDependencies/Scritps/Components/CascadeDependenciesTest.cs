using IdaelDev.DependencyInjection;
using UnityEngine;

namespace DependencyInjection.Samples._07_CascadeDependencies
{
    public class CascadeDependenciesTest : MonoBehaviour
    {
        [Inject] private IServiceA _serviceA;
        [Inject] private ILogger _logger;

        private void Start()
        {
            _logger?.Log("\n=== Testing Cascade Dependencies ===");

            if (_serviceA != null)
            {
                string result = _serviceA.GetData();
                _logger?.Log($"✓ Cascade resolved: {result}");
                _logger?.Log("✓ Order: Logger → ServiceC → ServiceB → ServiceA");
            }
            else
            {
                _logger?.Log("✗ Cascade dependency resolution FAILED");
            }

            _logger?.Log("===================\n");
        }

        private void OnGUI()
        {
            GUILayout.BeginArea(new Rect(10, 10, 400, 150));
            GUILayout.Label("=== Cascade Dependencies Test ===");
            GUILayout.Label("ServiceA → ServiceB → ServiceC → Logger");
            GUILayout.Label($"Result: {(_serviceA != null ? "✓ PASS" : "✗ FAIL")}");
            if (_serviceA != null)
            {
                GUILayout.Label($"Data: {_serviceA.GetData()}");
            }
            GUILayout.EndArea();
        }
    }
}
