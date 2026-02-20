using UnityEngine;

namespace DependencyInjection.Samples._02_ConstructorInjection
{
    public class ConstructorInjectionTest : MonoBehaviour
    {
        [Inject] private ILogger _logger;
        [Inject] private IGameManager _gameManager;

        private void Start()
        {
            _logger?.Log("\n=== Testing Constructor Injection ===");

            TestConstructorInjection();
            TestCascadeDependencies();

            _logger?.Log("=== All Tests Complete ===\n");
        }

        private void TestConstructorInjection()
        {
            if (_gameManager != null)
            {
                _logger.Log("✓ GameManager injected via constructor");
                _gameManager.StartGame();
            }
            else
            {
                Debug.LogError("✗ Constructor injection FAILED");
            }
        }

        private void TestCascadeDependencies()
        {
            // GameManager dépend de ILogger et IAudioService
            // Si ça fonctionne, c'est que les dépendances en cascade sont résolues
            if (_gameManager is { IsRunning: true })
            {
                _logger.Log("✓ Cascade dependencies resolved correctly");
            }
        }

        private void OnGUI()
        {
            GUILayout.BeginArea(new Rect(10, 10, 400, 200));
            GUILayout.Label("=== Constructor Injection Test ===");
            GUILayout.Label($"GameManager: {(_gameManager != null ? "✓ PASS" : "✗ FAIL")}");
            GUILayout.Label($"Cascade Dependencies: {(_gameManager?.IsRunning == true ? "✓ PASS" : "✗ FAIL")}");
            GUILayout.EndArea();
        }
    }
}
