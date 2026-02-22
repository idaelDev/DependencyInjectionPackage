using IdaelDev.DependencyInjection;
using UnityEngine;

namespace DependencyInjection.Samples._06_RuntimeInstantiation
{
    public class TestPrefabComponent : MonoBehaviour
    {
        [Inject] private ILogger _logger;

        private void Start()
        {
            _logger?.Log("✓ Prefab component injected!");
        }
    }
}
