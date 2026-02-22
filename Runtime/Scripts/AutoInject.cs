using System.Threading.Tasks;
using UnityEngine;

namespace IdaelDev.DependencyInjection
{
    /// <summary>
    /// Composant qui injecte automatiquement les dépendances lors de l'instanciation d'un GameObject.
    /// Ajoutez ce composant sur vos prefabs pour activer l'injection automatique.
    /// </summary>
    public class AutoInject : MonoBehaviour
    {
        [SerializeField] private bool injectOnAwake = true;
        [SerializeField] private bool injectChildren;

        private void Awake()
        {
            if (injectOnAwake)
            {
                _= Inject();
            }
        }

        /// <summary>
        /// Injecte les dépendances manuellement (supporte automatiquement [InjectAsync])
        /// </summary>
        private async Task Inject()
        {
            if (DIContext.Container == null)
            {
                Debug.LogError("DIContext.Container is null. Make sure DIContext is in the scene.");
                return;
            }

            if (injectChildren)
            {
                var components = GetComponentsInChildren<MonoBehaviour>(true);
                foreach (var component in components)
                {
                    await DIContext.Container.InjectComponentAsync(component);
                }
            }
            else
            {
                var components = GetComponents<MonoBehaviour>();
                foreach (var component in components)
                {
                    await DIContext.Container.InjectComponentAsync(component);
                }
            }
        }
    }
}
