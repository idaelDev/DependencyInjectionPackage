using System;
using System.Collections.Generic;
using System.Linq;

namespace DependencyInjection
{
    /// <summary>
    /// Informations sur une instance créée par le container
    /// </summary>
    public class InstanceInfo
    {
        public object Instance { get; set; }
        public Type ServiceType { get; set; }
        public Type ImplementationType { get; set; }
        public Lifetime Lifetime { get; set; }
        public List<Type> Dependencies { get; set; } = new();
        public string ScopeName { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public bool IsFromScene { get; set; }
    }

    /// <summary>
    /// Tracker pour suivre toutes les instances créées par le container
    /// </summary>
    public class DIContainerTracker
    {
        private static DIContainerTracker _instance;
        private readonly Dictionary<object, InstanceInfo> _instances = new Dictionary<object, InstanceInfo>();
        private readonly Dictionary<DIContainer, string> _scopeNames = new Dictionary<DIContainer, string>();
        private int _scopeCounter;

        public static DIContainerTracker Instance
        {
            get
            {
                _instance ??= new DIContainerTracker();
                return _instance;
            }
        }

        /// <summary>
        /// Enregistre une nouvelle instance
        /// </summary>
        public void RegisterInstance(
            object instance,
            Type serviceType,
            Type implementationType,
            Lifetime lifetime,
            DIContainer container,
            List<Type> dependencies,
            bool isFromScene = false)
        {
            if (instance == null) return;

            var info = new InstanceInfo
            {
                Instance = instance,
                ServiceType = serviceType,
                ImplementationType = implementationType,
                Lifetime = lifetime,
                Dependencies = dependencies ?? new List<Type>(),
                ScopeName = GetScopeName(container),
                IsFromScene = isFromScene
            };

            lock (_instances)
            {
                _instances[instance] = info;
            }
        }

        /// <summary>
        /// Enregistre un scope
        /// </summary>
        public void RegisterScope(DIContainer scope, string name = null)
        {
            if (!_scopeNames.ContainsKey(scope))
            {
                _scopeNames[scope] = name ?? $"Scope-{++_scopeCounter}";
            }
        }

        /// <summary>
        /// Obtient le nom d'un scope
        /// </summary>
        private string GetScopeName(DIContainer container)
        {
            return _scopeNames.GetValueOrDefault(container, "Root");
        }

        /// <summary>
        /// Désenregistre une instance
        /// </summary>
        public void UnregisterInstance(object instance)
        {
            if (instance == null) return;

            lock (_instances)
            {
                _instances.Remove(instance);
            }
        }

        /// <summary>
        /// Nettoie les instances d'un scope
        /// </summary>
        public void CleanupScope(DIContainer scope)
        {
            var scopeName = GetScopeName(scope);

            lock (_instances)
            {
                var toRemove = _instances
                    .Where(kvp => kvp.Value.ScopeName == scopeName)
                    .Select(kvp => kvp.Key)
                    .ToList();

                foreach (var key in toRemove)
                {
                    _instances.Remove(key);
                }
            }

            _scopeNames.Remove(scope);
        }

        /// <summary>
        /// Obtient toutes les instances
        /// </summary>
        public IEnumerable<InstanceInfo> GetAllInstances()
        {
            lock (_instances)
            {
                return _instances.Values.ToList();
            }
        }

        /// <summary>
        /// Obtient les instances par lifetime
        /// </summary>
        public IEnumerable<InstanceInfo> GetInstancesByLifetime(Lifetime lifetime)
        {
            lock (_instances)
            {
                return _instances.Values.Where(i => i.Lifetime == lifetime).ToList();
            }
        }

        /// <summary>
        /// Obtient les instances par scope
        /// </summary>
        public IEnumerable<InstanceInfo> GetInstancesByScope(string scopeName)
        {
            lock (_instances)
            {
                return _instances.Values.Where(i => i.ScopeName == scopeName).ToList();
            }
        }

        /// <summary>
        /// Obtient tous les noms de scopes actifs
        /// </summary>
        public IEnumerable<string> GetActiveScopeNames()
        {
            lock (_instances)
            {
                return _instances.Values.Select(i => i.ScopeName).Distinct().ToList();
            }
        }

        /// <summary>
        /// Obtient le nombre total d'instances
        /// </summary>
        public int GetTotalInstanceCount()
        {
            lock (_instances)
            {
                return _instances.Count;
            }
        }

        /// <summary>
        /// Nettoie tout
        /// </summary>
        public void Clear()
        {
            lock (_instances)
            {
                _instances.Clear();
            }
            _scopeNames.Clear();
            _scopeCounter = 0;
        }
    }
}
