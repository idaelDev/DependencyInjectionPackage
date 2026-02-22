using UnityEngine;

namespace IdaelDev.DependencyInjection.Samples._01_BasicInjection
{
    /// <summary>
    /// Test de l'injection de base : field et property
    /// </summary>
    public class BasicInjectionTest : MonoBehaviour
    {
        // Test injection de field privé
        [Inject] private ILogger _loggerField;

        // Test injection de property publique
        [Inject] public ILogger LoggerProperty { get; set; }

        private void Start()
        {
            TestFieldInjection();
            TestPropertyInjection();
            DisplayResults();
        }

        private void TestFieldInjection()
        {
            if (_loggerField != null)
            {
                _loggerField.Log("✓ Field injection works!");
            }
            else
            {
                Debug.LogError("✗ Field injection FAILED");
            }
        }

        private void TestPropertyInjection()
        {
            if (LoggerProperty != null)
            {
                LoggerProperty.Log("✓ Property injection works!");
            }
            else
            {
                Debug.LogError("✗ Property injection FAILED");
            }
        }

        private void DisplayResults()
        {
            Debug.Log("\n=== Basic Injection Test Results ===");
            Debug.Log($"Field Injection: {(_loggerField != null ? "PASS ✓" : "FAIL ✗")}");
            Debug.Log($"Property Injection: {(LoggerProperty != null ? "PASS ✓" : "FAIL ✗")}");
            Debug.Log("====================================\n");
        }

        private void OnGUI()
        {
            GUILayout.BeginArea(new Rect(10, 10, 400, 200));
            GUILayout.Label("=== Basic Injection Test ===");
            GUILayout.Label($"Field Injection: {(_loggerField != null ? "✓ PASS" : "✗ FAIL")}");
            GUILayout.Label($"Property Injection: {(LoggerProperty != null ? "✓ PASS" : "✗ FAIL")}");
            GUILayout.EndArea();
        }
    }
}
