using System.Collections;
using IdaelDev.DependencyInjection;
using UnityEngine;

namespace DependencyInjection.Samples._06_RuntimeInstantiation
{
    public class RuntimeInstantiationTest : MonoBehaviour
    {
        [SerializeField] private GameObject prefabWithAutoInject;

        [Inject] private ILogger _logger;

        private void Start()
        {
            _logger?.Log("Press SPACE to spawn prefab, A to add component");

            StartCoroutine(TestRuntimeInstanciationCrt());
        }


        IEnumerator TestRuntimeInstanciationCrt()
        {
            _logger.Log("Spawn Prefab in 3sec...");
            yield return new WaitForSeconds(3);
            SpawnPrefab();

            _logger.Log("AddComponent in 3sec...");
            yield return new WaitForSeconds(3);
            AddComponent();

        }

        private void SpawnPrefab()
        {
            if (prefabWithAutoInject != null)
            {
                prefabWithAutoInject.InstantiateWithInjection();
                _logger?.Log("Prefab spawned with injection");
            }
        }

        private void AddComponent()
        {
            var obj = new GameObject("DynamicObject");
            obj.AddComponentWithInjection<TestPrefabComponent>();
            _logger?.Log("Component added with injection");
        }

        private void OnGUI()
        {
            GUILayout.BeginArea(new Rect(10, 10, 400, 100));
            GUILayout.Label("=== Runtime Instantiation Test ===");
            GUILayout.Label("SPACE - Spawn Prefab");
            GUILayout.Label("A - Add Component");
            GUILayout.EndArea();
        }
    }
}
