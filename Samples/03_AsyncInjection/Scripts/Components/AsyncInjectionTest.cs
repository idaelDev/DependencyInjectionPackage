using System.Threading.Tasks;
using IdaelDev.DependencyInjection;
using UnityEngine;

namespace DependencyInjection.Samples._03_AsyncInjection
{
    public class AsyncInjectionTest : MonoBehaviour
    {
        [Inject] private ILogger _logger;
        private string _loadedData;
        private bool _isLoading;

        [InjectAsync]
        private async Task LoadDataAsync(IDataService dataService)
        {
            _isLoading = true;
            _logger.Log("Starting async injection test...");

            try
            {
                _loadedData = await dataService.LoadDataAsync();
                _logger.Log($"✓ Data loaded: {_loadedData}");
            }
            catch (System.Exception ex)
            {
                _logger.Log($"✗ Async injection FAILED: {ex.Message}");
            }
            finally
            {
                _isLoading = false;
            }
        }

        private void OnGUI()
        {
            GUILayout.BeginArea(new Rect(10, 10, 400, 200));
            GUILayout.Label("=== Async Injection Test ===");

            if (_isLoading)
            {
                GUILayout.Label("Status: Loading...");
            }
            else if (!string.IsNullOrEmpty(_loadedData))
            {
                GUILayout.Label($"Status: ✓ PASS");
                GUILayout.Label($"Data: {_loadedData}");
            }
            else
            {
                GUILayout.Label("Status: Waiting...");
            }

            GUILayout.EndArea();
        }
    }
}
