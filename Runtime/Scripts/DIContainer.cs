using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using UnityEngine;

namespace IdaelDev.DependencyInjection
{
    public enum Lifetime
    {
        Singleton, // Une seule instance partagée
        Transient, // Nouvelle instance à chaque fois
        Scoped // Une instance par scope
    }

    public class DIContainer : IDisposable
    {
        private readonly Dictionary<Type, ServiceDescriptor> _services = new();
        private readonly Dictionary<Type, object> _singletonInstances = new();
        private readonly Dictionary<Type, object> _scopedInstances = new();
        private readonly HashSet<Type> _resolvingTypes = new();
        private readonly HashSet<object> _asyncInitializedObjects = new();
        private readonly DIContainer _parentContainer;
        private readonly bool _isScope;
        private bool _isDisposed;

        public DIContainer()
        {
            _parentContainer = null;
            _isScope = false;
        }

        private DIContainer(DIContainer parent)
        {
            _parentContainer = parent;
            _isScope = true;
            // Hérite des services du parent
            foreach (var kvp in parent._services)
            {
                _services[kvp.Key] = kvp.Value;
            }
        }

        #region Registration

        public DIContainer Register<TInterface, TImplementation>(Lifetime lifetime = Lifetime.Transient)
            where TImplementation : TInterface
        {
            return Register(typeof(TInterface), typeof(TImplementation), lifetime);
        }

        public DIContainer Register<TImplementation>(Lifetime lifetime = Lifetime.Transient)
        {
            return Register(typeof(TImplementation), typeof(TImplementation), lifetime);
        }

        private DIContainer Register(Type serviceType, Type implementationType, Lifetime lifetime = Lifetime.Transient)
        {
            if (_isScope)
            {
                throw new InvalidOperationException(
                    "Cannot register services in a scoped container. Register in the root container.");
            }

            _services[serviceType] = new ServiceDescriptor
            {
                ServiceType = serviceType,
                ImplementationType = implementationType,
                Lifetime = lifetime,
                Factory = null
            };

            return this;
        }

        public DIContainer RegisterInstance<TInterface>(TInterface instance)
        {
            if (_isScope)
            {
                throw new InvalidOperationException(
                    "Cannot register instances in a scoped container. Register in the root container.");
            }

            _services[typeof(TInterface)] = new ServiceDescriptor
            {
                ServiceType = typeof(TInterface),
                ImplementationType = typeof(TInterface),
                Lifetime = Lifetime.Singleton,
                Factory = null
            };

            _singletonInstances[typeof(TInterface)] = instance;
            return this;
        }

        public DIContainer RegisterFactory<TInterface>(Func<DIContainer, TInterface> factory,
            Lifetime lifetime = Lifetime.Transient)
        {
            if (_isScope)
            {
                throw new InvalidOperationException(
                    "Cannot register factories in a scoped container. Register in the root container.");
            }

            _services[typeof(TInterface)] = new ServiceDescriptor
            {
                ServiceType = typeof(TInterface),
                ImplementationType = typeof(TInterface),
                Lifetime = lifetime,
                Factory = container => factory(container)
            };

            return this;
        }

        #endregion

        #region Resolution

        public T Resolve<T>()
        {
            return (T)Resolve(typeof(T));
        }

        private object Resolve(Type serviceType)
        {
            if (_isDisposed)
            {
                throw new ObjectDisposedException(nameof(DIContainer));
            }

            // Vérifier les cycles de dépendances
            if (_resolvingTypes.Contains(serviceType))
            {
                var chain = string.Join(" -> ", _resolvingTypes.Select(t => t.Name));
                throw new InvalidOperationException(
                    $"Circular dependency detected: {chain} -> {serviceType.Name}");
            }

            // Chercher le service (d'abord dans ce conteneur, puis dans le parent)
            if (!_services.TryGetValue(serviceType, out var descriptor))
            {
                if (_parentContainer != null)
                {
                    return _parentContainer.Resolve(serviceType);
                }

                throw new InvalidOperationException(
                    $"Service of type '{serviceType.FullName}' is not registered. " +
                    $"Make sure to call container.Register<{serviceType.Name}>() in ConfigureServices.");
            }

            // Gérer selon le lifetime
            switch (descriptor.Lifetime)
            {
                case Lifetime.Singleton:
                    return ResolveSingleton(descriptor);

                case Lifetime.Scoped:
                    return ResolveScoped(descriptor);

                case Lifetime.Transient:
                    return ResolveTransient(descriptor);

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public async Task<T> ResolveAsync<T>()
        {
            return (T)await ResolveAsync(typeof(T));
        }

        private async Task<object> ResolveAsync(Type serviceType)
        {
            var instance = Resolve(serviceType);

            // Éviter de réinitialiser les objets déjà traités
            if (_asyncInitializedObjects.Contains(instance))
            {
                return instance;
            }

            // Injecter les dépendances asynchrones si nécessaire
            await InjectAsyncDependencies(instance);
            _asyncInitializedObjects.Add(instance);

            return instance;
        }

        private object ResolveSingleton(ServiceDescriptor descriptor)
        {
            // Vérifier d'abord dans le root container
            var rootContainer = GetRootContainer();

            if (rootContainer._singletonInstances.TryGetValue(descriptor.ServiceType, out var instance))
            {
                return instance;
            }

            instance = CreateInstance(descriptor);
            rootContainer._singletonInstances[descriptor.ServiceType] = instance;
            return instance;
        }

        private object ResolveScoped(ServiceDescriptor descriptor)
        {
            if (_scopedInstances.TryGetValue(descriptor.ServiceType, out var instance))
            {
                return instance;
            }

            instance = CreateInstance(descriptor);
            _scopedInstances[descriptor.ServiceType] = instance;
            return instance;
        }

        private object ResolveTransient(ServiceDescriptor descriptor)
        {
            return CreateInstance(descriptor);
        }

        private object CreateInstance(ServiceDescriptor descriptor)
        {
            _resolvingTypes.Add(descriptor.ServiceType);

            try
            {
                // Si une factory est définie, l'utiliser
                if (descriptor.Factory != null)
                {
                    return descriptor.Factory(this);
                }

                // Trouver le constructeur avec le plus de paramètres injectable
                var constructor = GetBestConstructor(descriptor.ImplementationType);
                var parameters = constructor.GetParameters();
                var parameterInstances = new object[parameters.Length];
                var dependencies = new List<Type>();
                for (int i = 0; i < parameters.Length; i++)
                {
                    parameterInstances[i] = Resolve(parameters[i].ParameterType);
                    dependencies.Add(parameters[i].ParameterType);
                }

                var instance = Activator.CreateInstance(descriptor.ImplementationType, parameterInstances);

                // Injecter les propriétés
                InjectProperties(instance);
                TrackInstance(instance, descriptor, dependencies);
                return instance;
            }
            finally
            {
                _resolvingTypes.Remove(descriptor.ServiceType);
            }
        }

        private ConstructorInfo GetBestConstructor(Type type)
        {
            // MonoBehaviour ne peut pas avoir d'injection par constructeur
            if (typeof(MonoBehaviour).IsAssignableFrom(type))
            {
                throw new InvalidOperationException(
                    $"Cannot use constructor injection for MonoBehaviour '{type.Name}'. " +
                    $"MonoBehaviour instances must be created by Unity. " +
                    $"Use [Inject] on properties or fields instead.");
            }

            var constructors = type.GetConstructors(BindingFlags.Public | BindingFlags.Instance);

            if (constructors.Length == 0)
            {
                throw new InvalidOperationException($"No public constructor found for type {type.Name}");
            }

            // Prioriser les constructeurs avec [Inject]
            var injectConstructor = constructors.FirstOrDefault(c => c.GetCustomAttribute<InjectAttribute>() != null);
            if (injectConstructor != null)
            {
                return injectConstructor;
            }

            // Sinon, prendre celui avec le plus de paramètres
            return constructors.OrderByDescending(c => c.GetParameters().Length).First();
        }

        #endregion

        #region Property and Field Injection

        private void InjectProperties(object instance)
        {
            if (instance == null)
            {
                return;
            }

            var type = instance.GetType();
            var injectedDependencies = new List<Type>();

            // Injecter les propriétés
            var properties = type
                .GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(p => p.GetCustomAttribute<InjectAttribute>() != null);

            foreach (var property in properties)
            {
                if (!property.CanWrite)
                {
                    Debug.LogWarning(
                        $"Property '{property.Name}' on type '{type.Name}' has [Inject] but is read-only. Skipping.");
                    continue;
                }

                try
                {
                    var value = Resolve(property.PropertyType);
                    property.SetValue(instance, value);
                    injectedDependencies.Add(property.PropertyType);
                }
                catch (Exception ex)
                {
                    Debug.LogError($"Failed to inject property '{property.Name}' on type '{type.Name}': {ex.Message}");
                    throw;
                }
            }

            // Injecter les fields
            var fields = type
                .GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(f => f.GetCustomAttribute<InjectAttribute>() != null);

            foreach (var field in fields)
            {
                if (field.IsInitOnly)
                {
                    Debug.LogWarning(
                        $"Field '{field.Name}' on type '{type.Name}' has [Inject] but is readonly. Skipping.");
                    continue;
                }

                try
                {
                    var value = Resolve(field.FieldType);
                    field.SetValue(instance, value);
                    injectedDependencies.Add(field.FieldType);
                }
                catch (Exception ex)
                {
                    Debug.LogError($"Failed to inject field '{field.Name}' on type '{type.Name}': {ex.Message}");
                    throw;
                }
            }

            if (instance is MonoBehaviour && injectedDependencies.Count > 0)
            {
                TrackSceneInstance(instance, type); // ← AJOUT
            }
        }

        private async Task InjectAsyncDependencies(object instance)
        {
            if (instance == null)
            {
                return;
            }

            var methods = instance.GetType()
                .GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(m => m.GetCustomAttribute<InjectAsyncAttribute>() != null);

            foreach (var method in methods)
            {
                var parameters = method.GetParameters();
                var parameterInstances = new object[parameters.Length];

                for (int i = 0; i < parameters.Length; i++)
                {
                    // Utiliser Resolve normal ici pour éviter la récursion
                    parameterInstances[i] = await ResolveAsync(parameters[i].ParameterType);
                }

                try
                {
                    var result = method.Invoke(instance, parameterInstances);

                    // Si la méthode retourne une Task, l'attendre
                    if (result is Task task)
                    {
                        await task;
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogError($"Failed to invoke [InjectAsync] method '{method.Name}': {ex.Message}");
                    throw;
                }
            }
        }

        #endregion

        #region Scoping

        public DIContainer CreateScope()
        {
            var scope = new DIContainer(GetRootContainer());

#if UNITY_EDITOR
            DIContainerTracker.Instance.RegisterScope(scope);
#endif

            return scope;
        }

        private DIContainer GetRootContainer()
        {
            var current = this;
            while (current._parentContainer != null)
            {
                current = current._parentContainer;
            }

            return current;
        }

        public void Dispose()
        {
            if (_isDisposed)
            {
                return;
            }

            _isDisposed = true;
#if UNITY_EDITOR
            if (_isScope)
            {
                DIContainerTracker.Instance.CleanupScope(this);
            }
            #endif
            if (_isScope)
            {
                // Disposer les instances scoped
                foreach (var instance in _scopedInstances.Values)
                {
                    if (instance is IDisposable disposable)
                    {
                        try
                        {
                            disposable.Dispose();
                        }
                        catch (Exception ex)
                        {
                            Debug.LogError($"Error disposing scoped instance: {ex.Message}");
                        }
                    }
                }

                _scopedInstances.Clear();
            }
            else
            {
                // Pour le root container, disposer aussi les singletons
                foreach (var instance in _singletonInstances.Values)
                {
                    if (instance is IDisposable disposable)
                    {
                        try
                        {
                            disposable.Dispose();
                        }
                        catch (Exception ex)
                        {
                            Debug.LogError($"Error disposing singleton instance: {ex.Message}");
                        }
                    }
                }

                _singletonInstances.Clear();
            }

            _resolvingTypes.Clear();
            _asyncInitializedObjects.Clear();
        }

        #endregion

        #region Unity Integration

        public void InjectGameObject(GameObject gameObject)
        {
            var components = gameObject.GetComponents<MonoBehaviour>();
            foreach (var component in components)
            {
                InjectComponent(component);
            }
        }

        public async Task InjectGameObjectAsync(GameObject gameObject)
        {
            var components = gameObject.GetComponents<MonoBehaviour>();
            foreach (var component in components)
            {
                await InjectComponentAsync(component);
            }
        }

        public void InjectComponent(MonoBehaviour component)
        {
            InjectProperties(component);
        }

        public async Task InjectComponentAsync(MonoBehaviour component)
        {
            InjectProperties(component);
            await InjectAsyncDependencies(component);
        }

        #endregion

        #region Validation and Debugging

        /// <summary>
        /// Vérifie si un service est enregistré
        /// </summary>
        public bool IsRegistered<T>()
        {
            return IsRegistered(typeof(T));
        }

        /// <summary>
        /// Vérifie si un service est enregistré
        /// </summary>
        private bool IsRegistered(Type serviceType)
        {
            if (_services.ContainsKey(serviceType))
            {
                return true;
            }

            return _parentContainer?.IsRegistered(serviceType) ?? false;
        }

        /// <summary>
        /// Retourne tous les services enregistrés
        /// </summary>
        public IEnumerable<Type> GetRegisteredServices()
        {
            var services = new HashSet<Type>(_services.Keys);

            if (_parentContainer != null)
            {
                foreach (var service in _parentContainer.GetRegisteredServices())
                {
                    services.Add(service);
                }
            }

            return services;
        }

        /// <summary>
        /// Valide que toutes les dépendances d'un type peuvent être résolues
        /// </summary>
        public bool ValidateDependencies(Type type, out string error)
        {
            error = null;

            try
            {
                if (typeof(MonoBehaviour).IsAssignableFrom(type))
                {
                    // Pour MonoBehaviour, vérifier les propriétés et fields
                    var members = type
                        .GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                        .Where(p => p.GetCustomAttribute<InjectAttribute>() != null)
                        .Select(p => new { Type = p.PropertyType, p.Name })
                        .Concat(
                            type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                                .Where(f => f.GetCustomAttribute<InjectAttribute>() != null)
                                .Select(f => new { Type = f.FieldType, f.Name })
                        );

                    foreach (var member in members)
                    {
                        if (!IsRegistered(member.Type))
                        {
                            error = $"Dependency '{member.Type.Name}' for member '{member.Name}' is not registered";
                            return false;
                        }
                    }
                }
                else
                {
                    // Pour les classes normales, vérifier le constructeur
                    var constructor = GetBestConstructor(type);
                    foreach (var param in constructor.GetParameters())
                    {
                        if (!IsRegistered(param.ParameterType))
                        {
                            error =
                                $"Dependency '{param.ParameterType.Name}' for parameter '{param.Name}' is not registered";
                            return false;
                        }
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                error = ex.Message;
                return false;
            }
        }

        #endregion

        #region Tracking

        /// <summary>
        /// Enregistre une instance dans le tracker pour le monitoring
        /// </summary>
        private void TrackInstance(object instance, ServiceDescriptor descriptor, List<Type> dependencies = null)
        {
            if (instance == null) return;

#if UNITY_EDITOR
            try
            {
                DIContainerTracker.Instance.RegisterInstance(
                    instance,
                    descriptor.ServiceType,
                    descriptor.ImplementationType,
                    descriptor.Lifetime,
                    this,
                    dependencies
                );
            }
            catch
            {
                // Ignore tracking errors in production
            }
            #endif
        }

        /// <summary>
        /// Enregistre une instance de scène dans le tracker
        /// </summary>
        private void TrackSceneInstance(object instance, Type implementationType)
        {
#if UNITY_EDITOR
            try
            {
                DIContainerTracker.Instance.RegisterInstance(
                    instance,
                    implementationType,
                    implementationType,
                    Lifetime.Singleton, // Scene objects are treated as singletons
                    this,
                    null,
                    true // isFromScene
                );
            }
            catch
            {
                // Ignore tracking errors
            }
#endif
        }

        #endregion
    }

    internal class ServiceDescriptor
    {
        public Type ServiceType { get; set; }
        public Type ImplementationType { get; set; }
        public Lifetime Lifetime { get; set; }
        public Func<DIContainer, object> Factory { get; set; }
    }
}
