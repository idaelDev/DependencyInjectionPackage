using UnityEngine;

namespace DependencyInjection
{
    /// <summary>
    /// Composant qui injecte automatiquement les dépendances lors de l'instanciation d'un GameObject.
    /// Ajoutez ce composant sur vos prefabs pour activer l'injection automatique.
    /// </summary>
    public class AutoInject : MonoBehaviour
    {
        [SerializeField] private bool _injectOnAwake = true;
        [SerializeField] private bool _injectChildren = false;

        private void Awake()
        {
            if (_injectOnAwake)
            {
                Inject();
            }
        }

        /// <summary>
        /// Injecte les dépendances manuellement (supporte automatiquement [InjectAsync])
        /// </summary>
        public async void Inject()
        {
            if (DIContext.Container == null)
            {
                Debug.LogError("DIContext.Container is null. Make sure DIContext is in the scene.");
                return;
            }

            if (_injectChildren)
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
