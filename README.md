# Système d'Injection de Dépendances pour Unity

Un système d'IoC (Inversion of Control) Container complet pour Unity avec support de l'injection de dépendances, des cycles de vie multiples, de l'asynchrone et des scopes.

## 🚀 Fonctionnalités

- ✅ **Injection par constructeur** - Résolution automatique des dépendances
- ✅ **Injection par propriété** - Avec l'attribut `[Inject]`
- ✅ **Injection asynchrone** - Support des dépendances async avec `[InjectAsync]`
- ✅ **Cycles de vie multiples** - Singleton, Transient, Scoped
- ✅ **Résolution en cascade** - Gestion automatique des dépendances imbriquées
- ✅ **Détection de cycles** - Prévention des dépendances circulaires
- ✅ **Scopes temporaires** - Pour des IoC containers isolés
- ✅ **Injection automatique au runtime** - GameObjects instanciés dynamiquement
- ✅ **Extensions Unity** - Méthodes pratiques pour Instantiate, AddComponent, etc.
- ✅ **Support MonoBehaviour** - Injection dans les composants Unity

## 📦 Installation

1. Copiez le dossier `DependencyInjection` dans votre projet Unity
2. Créez une classe héritant de `DIContext` pour configurer vos services
3. Ajoutez votre contexte à un GameObject dans votre scène

## 🎯 Utilisation Rapide

### 1. Créer votre contexte DI

```csharp
using DependencyInjection;
using UnityEngine;

public class MyGameContext : DIContext
{
    protected override void ConfigureServices(DIContainer container)
    {
        // Singleton - partagé dans toute l'application
        container.Register<ILogger, ConsoleLogger>(Lifetime.Singleton);
        container.Register<IGameManager, GameManager>(Lifetime.Singleton);

        // Transient - nouvelle instance à chaque résolution
        container.Register<IWeaponFactory, WeaponFactory>(Lifetime.Transient);

        // Scoped - une instance par scope
        container.Register<IDatabaseTransaction, DatabaseTransaction>(Lifetime.Scoped);

        // Factory personnalisée
        container.RegisterFactory<IConfig>(c => 
            new Config { Level = 10 }, 
            Lifetime.Singleton
        );
    }
}
```

### 2. Définir vos services

```csharp
// Interface
public interface ILogger
{
    void Log(string message);
}

// Implémentation avec injection par constructeur
public class ConsoleLogger : ILogger
{
    public void Log(string message)
    {
        Debug.Log(message);
    }
}

// Service avec dépendances
public class GameManager : IGameManager
{
    private readonly ILogger _logger;
    private readonly IWeaponFactory _weaponFactory;

    // Le constructeur avec [Inject] sera utilisé pour la résolution
    [Inject]
    public GameManager(ILogger logger, IWeaponFactory weaponFactory)
    {
        _logger = logger;
        _weaponFactory = weaponFactory;
    }

    public void StartGame()
    {
        _logger.Log("Game started!");
    }
}
```

### 3. Injection dans les MonoBehaviour

```csharp
public class PlayerController : MonoBehaviour
{
    // Injection par propriété
    [Inject] 
    public ILogger Logger { get; set; }

    [Inject]
    public IGameManager GameManager { get; set; }

    private void Start()
    {
        Logger.Log("PlayerController initialized");
        GameManager.StartGame();
    }
}
```

### 4. Injection asynchrone

```csharp
public class AsyncDataLoader : MonoBehaviour
{
    [Inject]
    public ILogger Logger { get; set; }

    private string _loadedData;

    // Méthode appelée automatiquement après l'injection
    [InjectAsync]
    private async Task InitializeAsync(IDataService dataService)
    {
        Logger.Log("Loading data...");
        _loadedData = await dataService.LoadDataAsync();
        Logger.Log("Data loaded!");
    }
}
```

## 🔄 Cycles de Vie

### Singleton
Une seule instance créée et partagée dans toute l'application.

```csharp
container.Register<ILogger, ConsoleLogger>(Lifetime.Singleton);
```

### Transient
Une nouvelle instance est créée à chaque résolution.

```csharp
container.Register<IWeapon, Sword>(Lifetime.Transient);
```

### Scoped
Une instance par scope. Utile pour des opérations temporaires ou des transactions.

```csharp
container.Register<ITransaction, Transaction>(Lifetime.Scoped);

// Utilisation
using (var scope = DIContext.Instance.CreateScope())
{
    var transaction = scope.Resolve<ITransaction>();
    transaction.Begin();
    // ...
    transaction.Commit();
} // Dispose automatique des services Scoped
```

## 🎮 Instanciation Runtime

### Méthode 1: Composant AutoInject (Recommandé)

Ajoutez le composant `AutoInject` sur vos prefabs :

```csharp
// Sur votre prefab, cochez les options dans l'inspecteur
// - Inject On Awake: true
// - Inject Children: false (ou true pour injecter aussi les enfants)
// - Use Async: false (ou true si vous avez des dépendances async)

GameObject instance = Instantiate(myPrefab);
// Les dépendances sont automatiquement injectées !
```

### Méthode 2: Extensions Unity

```csharp
// Instanciation avec injection automatique
GameObject player = playerPrefab.InstantiateWithInjection();
GameObject enemy = enemyPrefab.InstantiateWithInjection(position, rotation);
GameObject item = itemPrefab.InstantiateWithInjection(parent);

// Ajouter un composant avec injection
var controller = gameObject.AddComponentWithInjection<PlayerController>();

// Injecter manuellement dans un objet existant
existingObject.InjectDependencies();
```

### Méthode 3: Injection manuelle

```csharp
GameObject instance = Instantiate(myPrefab);
DIContext.Container.InjectGameObject(instance);
```

## 🔗 Dépendances en Cascade

Le système résout automatiquement les dépendances imbriquées dans le bon ordre :

```csharp
// ProfileService dépend de UserService
// UserService dépend de AuthService  
// AuthService dépend de Logger

public class ProfileService : IProfileService
{
    private readonly IUserService _userService;
    
    public ProfileService(IUserService userService) // ← UserService sera résolu
    {
        _userService = userService;
    }
}

public class UserService : IUserService
{
    private readonly IAuthService _authService;
    
    public UserService(IAuthService authService) // ← AuthService sera résolu
    {
        _authService = authService;
    }
}

// Dans votre MonoBehaviour
[Inject]
public IProfileService ProfileService { get; set; } // ← Tout est résolu automatiquement !
```

## 🛡️ Détection de Cycles

Le système détecte automatiquement les dépendances circulaires :

```csharp
// ❌ Ceci lancera une exception
public class ServiceA
{
    public ServiceA(ServiceB b) { }
}

public class ServiceB
{
    public ServiceB(ServiceA a) { } // Cycle détecté !
}
```

## 🎯 Scopes Avancés

Utilisez les scopes pour isoler des opérations :

```csharp
public class DatabaseManager : MonoBehaviour
{
    [Inject]
    public ILogger Logger { get; set; }

    public void PerformTransaction()
    {
        using (var scope = DIContext.Instance.CreateScope())
        {
            // Services Scoped = une instance par scope
            var transaction = scope.Resolve<IDatabaseTransaction>();
            var validator = scope.Resolve<IDataValidator>();
            
            transaction.Begin();
            
            try
            {
                // Travail avec la transaction...
                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
            }
        } // Dispose automatique
    }
}
```

## 📝 Bonnes Pratiques

### 1. Utilisez des interfaces
```csharp
// ✅ Bon
container.Register<ILogger, ConsoleLogger>(Lifetime.Singleton);

// ❌ À éviter (couplage fort)
container.Register<ConsoleLogger>(Lifetime.Singleton);
```

### 2. Préférez l'injection par constructeur
```csharp
// ✅ Bon - dépendances explicites
public class GameManager
{
    private readonly ILogger _logger;
    
    public GameManager(ILogger logger)
    {
        _logger = logger;
    }
}

// ⚠️ Acceptable pour MonoBehaviour uniquement
public class PlayerController : MonoBehaviour
{
    [Inject] public ILogger Logger { get; set; }
}
```

### 3. Utilisez Singleton pour les services stateless
```csharp
// Services sans état partagé = Singleton
container.Register<ILogger, ConsoleLogger>(Lifetime.Singleton);
container.Register<IEventBus, EventBus>(Lifetime.Singleton);
```

### 4. Utilisez Transient pour les objets avec état
```csharp
// Objets avec état spécifique = Transient
container.Register<ICommand, AttackCommand>(Lifetime.Transient);
container.Register<IWeapon, Sword>(Lifetime.Transient);
```

### 5. Utilisez Scoped pour les transactions
```csharp
// Opérations temporaires = Scoped
container.Register<IDatabaseTransaction, Transaction>(Lifetime.Scoped);
container.Register<IHttpRequest, HttpRequest>(Lifetime.Scoped);
```

## 🔍 Résolution Manuelle

```csharp
// Résoudre un service manuellement
var logger = DIContext.Container.Resolve<ILogger>();

// Résolution asynchrone
var dataService = await DIContext.Container.ResolveAsync<IDataService>();
```

## ⚙️ Configuration du DIContext

```csharp
public class MyGameContext : DIContext
{
    // Inspecter ces paramètres dans Unity
    [SerializeField] private bool _dontDestroyOnLoad = true;
    [SerializeField] private bool _injectOnAwake = true;

    protected override void ConfigureServices(DIContainer container)
    {
        // Enregistrez tous vos services ici
        container.Register<ILogger, ConsoleLogger>(Lifetime.Singleton);
        
        // ...
    }
}
```

## 🧪 Exemple Complet

Voir le fichier `Examples/ExampleUsage.cs` pour des exemples complets incluant :
- Services simples et avec dépendances
- Injection asynchrone
- Scopes
- Instanciation runtime
- Dépendances en cascade
- Transactions

## 📋 API Référence

### DIContainer

| Méthode | Description |
|---------|-------------|
| `Register<TInterface, TImpl>(Lifetime)` | Enregistre un service |
| `RegisterInstance<T>(instance)` | Enregistre une instance existante |
| `RegisterFactory<T>(factory, Lifetime)` | Enregistre avec une factory |
| `Resolve<T>()` | Résout un service |
| `ResolveAsync<T>()` | Résout avec injection async |
| `CreateScope()` | Crée un nouveau scope |
| `InjectGameObject(GameObject)` | Injecte dans un GameObject |
| `InjectComponent(MonoBehaviour)` | Injecte dans un composant |

### Attributs

| Attribut | Utilisation |
|----------|-------------|
| `[Inject]` | Propriété ou constructeur à injecter |
| `[InjectAsync]` | Méthode d'initialisation async |

### Extensions Unity

| Méthode | Description |
|---------|-------------|
| `InstantiateWithInjection()` | Instantie avec injection auto |
| `AddComponentWithInjection<T>()` | Ajoute un composant avec injection |
| `InjectDependencies()` | Injecte dans un objet existant |

## 🐛 Troubleshooting

### "Service not registered"
Assurez-vous d'avoir enregistré le service dans `ConfigureServices()`.

### "Circular dependency detected"
Revoyez votre architecture - utilisez des événements ou le pattern Mediator.

### "DIContext.Container is null"
Vérifiez que DIContext est bien dans la scène et initialisé avant utilisation.

### Les dépendances ne sont pas injectées
- Vérifiez que l'attribut `[Inject]` est présent
- Pour les GameObjects runtime, utilisez `AutoInject` ou les extensions
- Assurez-vous que `InjectOnAwake` est activé dans DIContext

## 📄 Licence

Libre d'utilisation pour vos projets Unity personnels et commerciaux.
