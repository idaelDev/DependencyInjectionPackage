using UnityEngine;

namespace DependencyInjection
{
    /// <summary>
    /// Extensions pour faciliter l'injection dans Unity
    /// </summary>
    public static class UnityDIExtensions
    {
        /// <summary>
        /// Instancie un GameObject et injecte automatiquement ses dépendances
        /// </summary>
        public static GameObject InstantiateWithInjection(this GameObject prefab)
        {
            var instance = Object.Instantiate(prefab);
            DIContext.Container?.InjectGameObject(instance);
            return instance;
        }

        /// <summary>
        /// Instancie un GameObject avec position/rotation et injecte automatiquement ses dépendances
        /// </summary>
        public static GameObject InstantiateWithInjection(this GameObject prefab, Vector3 position, Quaternion rotation)
        {
            var instance = Object.Instantiate(prefab, position, rotation);
            DIContext.Container?.InjectGameObject(instance);
            return instance;
        }

        /// <summary>
        /// Instancie un GameObject avec parent et injecte automatiquement ses dépendances
        /// </summary>
        public static GameObject InstantiateWithInjection(this GameObject prefab, Transform parent)
        {
            var instance = Object.Instantiate(prefab, parent);
            DIContext.Container?.InjectGameObject(instance);
            return instance;
        }

        /// <summary>
        /// Instancie un GameObject avec tous les paramètres et injecte automatiquement ses dépendances
        /// </summary>
        public static GameObject InstantiateWithInjection(this GameObject prefab, Vector3 position, Quaternion rotation, Transform parent)
        {
            var instance = Object.Instantiate(prefab, position, rotation, parent);
            DIContext.Container?.InjectGameObject(instance);
            return instance;
        }

        /// <summary>
        /// Instancie un composant et injecte automatiquement ses dépendances
        /// </summary>
        public static T InstantiateWithInjection<T>(this T component) where T : Component
        {
            var instance = Object.Instantiate(component);
            DIContext.Container?.InjectGameObject(instance.gameObject);
            return instance;
        }

        /// <summary>
        /// Instancie un composant avec position/rotation et injecte automatiquement ses dépendances
        /// </summary>
        public static T InstantiateWithInjection<T>(this T component, Vector3 position, Quaternion rotation) where T : Component
        {
            var instance = Object.Instantiate(component, position, rotation);
            DIContext.Container?.InjectGameObject(instance.gameObject);
            return instance;
        }

        /// <summary>
        /// Instancie un composant avec parent et injecte automatiquement ses dépendances
        /// </summary>
        public static T InstantiateWithInjection<T>(this T component, Transform parent) where T : Component
        {
            var instance = Object.Instantiate(component, parent);
            DIContext.Container?.InjectGameObject(instance.gameObject);
            return instance;
        }

        /// <summary>
        /// Ajoute un composant à un GameObject et injecte automatiquement ses dépendances
        /// </summary>
        public static T AddComponentWithInjection<T>(this GameObject gameObject) where T : Component
        {
            var component = gameObject.AddComponent<T>();
            DIContext.Container?.InjectComponent(component as MonoBehaviour);
            return component;
        }

        /// <summary>
        /// Ajoute un composant à un GameObject et injecte automatiquement ses dépendances (supporte [InjectAsync])
        /// </summary>
        public static async System.Threading.Tasks.Task<T> AddComponentWithInjectionAsync<T>(this GameObject gameObject) where T : Component
        {
            var component = gameObject.AddComponent<T>();
            if (DIContext.Container != null)
            {
                await DIContext.Container.InjectComponentAsync(component as MonoBehaviour);
            }
            return component;
        }

        /// <summary>
        /// Injecte les dépendances dans un GameObject existant
        /// </summary>
        public static void InjectDependencies(this GameObject gameObject)
        {
            DIContext.Container?.InjectGameObject(gameObject);
        }

        /// <summary>
        /// Injecte les dépendances dans un GameObject existant (supporte [InjectAsync])
        /// </summary>
        public static async System.Threading.Tasks.Task InjectDependenciesAsync(this GameObject gameObject)
        {
            if (DIContext.Container != null)
            {
                await DIContext.Container.InjectGameObjectAsync(gameObject);
            }
        }

        /// <summary>
        /// Injecte les dépendances dans un Component existant
        /// </summary>
        public static void InjectDependencies(this MonoBehaviour component)
        {
            DIContext.Container?.InjectComponent(component);
        }

        /// <summary>
        /// Injecte les dépendances dans un Component existant (supporte [InjectAsync])
        /// </summary>
        public static async System.Threading.Tasks.Task InjectDependenciesAsync(this MonoBehaviour component)
        {
            if (DIContext.Container != null)
            {
                await DIContext.Container.InjectComponentAsync(component);
            }
        }
    }
}
