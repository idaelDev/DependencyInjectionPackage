using System.Threading.Tasks;
using UnityEngine;

namespace IdaelDev.DependencyInjection
{
    /// <summary>
    /// Contexte global d'injection de dépendances pour Unity.
    /// Doit être ajouté à un GameObject dans la scène pour gérer l'injection automatique.
    /// </summary>
    public class DIContext : MonoBehaviour
    {
        private static DIContext _instance;
        private DIContainer _container;

        [SerializeField] private bool dontDestroyOnLoad = true;
        [SerializeField] private bool injectOnAwake = true;

        private static DIContext Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindFirstObjectByType<DIContext>();
                    if (_instance == null)
                    {
                        Debug.LogWarning("No DIContext found in scene. Creating one automatically.");
                        var go = new GameObject("DIContext");
                        _instance = go.AddComponent<DIContext>();
                    }
                }
                return _instance;
            }
        }

        public static DIContainer Container => Instance._container;

        protected virtual void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
                if (dontDestroyOnLoad)
                {
                    DontDestroyOnLoad(gameObject);
                }
                InitializeContainer();
            }
            else if (_instance != this)
            {
                Debug.LogWarning("Multiple DIContext instances detected. Destroying duplicate.");
                Destroy(gameObject);
                return;
            }

            if (injectOnAwake)
            {
                _ = InjectSceneObjects();
            }
        }

        protected virtual void InitializeContainer()
        {
            _container = new DIContainer();
            ConfigureServices(_container);
        }

        /// <summary>
        /// Override this method to configure your services
        /// </summary>
        protected virtual void ConfigureServices(DIContainer container)
        {
            // À surcharger dans une classe dérivée pour enregistrer vos services
            Debug.Log("DIContext: No services configured. Override ConfigureServices to register services.");
        }

        /// <summary>
        /// Injecte les dépendances dans tous les MonoBehaviour de la scène
        /// </summary>
        private async Task InjectSceneObjects()
        {
            var allMonoBehaviours = FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            foreach (var behaviour in allMonoBehaviours)
            {
                if (behaviour != this) // Ne pas s'injecter soi-même
                {
                    await _container.InjectComponentAsync(behaviour);
                }
            }
        }

        /// <summary>
        /// Crée un scope temporaire pour des dépendances isolées
        /// </summary>
        public DIContainer CreateScope()
        {
            return _container.CreateScope();
        }

        private void OnDestroy()
        {
            if (_instance == this)
            {
                _container?.Dispose();
                _instance = null;
            }
        }
    }
}
