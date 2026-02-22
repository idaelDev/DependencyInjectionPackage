# Dependency Injection System for Unity

A complete IoC (Inversion of Control) Container system for Unity with support for dependency injection, multiple lifetimes, asynchronous operations, and scopes.

## Features

- **Constructor Injection** - Automatic dependency resolution  
- **Property Injection** - Using the `[Inject]` attribute  
- **Asynchronous Injection** - Support for async dependencies with `[InjectAsync]`  
- **Multiple Lifetimes** - Singleton, Transient, Scoped  
- **Cascading Resolution** - Automatic handling of nested dependencies  
- **Cycle Detection** - Prevention of circular dependencies  
- **Temporary Scopes** - For isolated IoC containers  
- **Automatic Runtime Injection** - Dynamically instantiated GameObjects  
- **Unity Extensions** - Convenient methods for Instantiate, AddComponent, etc.  
- **MonoBehaviour Support** - Injection into Unity components  

## Installation

1. Copy the `DependencyInjection` folder into your Unity project  
2. Create a class inheriting from `DIContext` to configure your services  
3. Add your context to a GameObject in your scene  

## Quick Start

### 1. Create Your DI Context

```csharp
using DependencyInjection;
using UnityEngine;

public class MyGameContext : DIContext
{
    protected override void ConfigureServices(DIContainer container)
    {
        // Singleton - shared across the entire application
        container.Register<ILogger, ConsoleLogger>(Lifetime.Singleton);
        container.Register<IGameManager, GameManager>(Lifetime.Singleton);

        // Transient - new instance on each resolution
        container.Register<IWeaponFactory, WeaponFactory>(Lifetime.Transient);

        // Scoped - one instance per scope
        container.Register<IDatabaseTransaction, DatabaseTransaction>(Lifetime.Scoped);

        // Custom factory
        container.RegisterFactory<IConfig>(c => 
            new Config { Level = 10 }, 
            Lifetime.Singleton
        );
    }
}
```

### 2. Define Your Services

```csharp
public interface ILogger
{
    void Log(string message);
}

public class ConsoleLogger : ILogger
{
    public void Log(string message)
    {
        Debug.Log(message);
    }
}

public class GameManager : IGameManager
{
    private readonly ILogger _logger;
    private readonly IWeaponFactory _weaponFactory;

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

### 3. Injection in MonoBehaviour

```csharp
public class PlayerController : MonoBehaviour
{
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

### 4. Asynchronous Injection

```csharp
public class AsyncDataLoader : MonoBehaviour
{
    [Inject]
    public ILogger Logger { get; set; }

    private string _loadedData;

    [InjectAsync]
    private async Task InitializeAsync(IDataService dataService)
    {
        Logger.Log("Loading data...");
        _loadedData = await dataService.LoadDataAsync();
        Logger.Log("Data loaded!");
    }
}
```

## Lifecycles

### Singleton

A single instance created and shared across the entire application.

```csharp
container.Register<ILogger, ConsoleLogger>(Lifetime.Singleton);
```

### Transient

A new instance is created on each resolution.

```csharp
container.Register<IWeapon, Sword>(Lifetime.Transient);
```

### Scoped

One instance per scope. Useful for temporary operations or transactions.

```csharp
container.Register<ITransaction, Transaction>(Lifetime.Scoped);

using (var scope = DIContext.Instance.CreateScope())
{
    var transaction = scope.Resolve<ITransaction>();
    transaction.Begin();
    transaction.Commit();
}
```

## Runtime Instantiation

### Method 1: AutoInject Component (Recommended)

Add the `AutoInject` component to your prefabs:

```csharp
// On your prefab, enable options in the inspector
// - Inject On Awake: true
// - Inject Children: false
// - Use Async: false

GameObject instance = Instantiate(myPrefab);
// Dependencies are automatically injected
```

### Method 2: Unity Extensions

```csharp
GameObject player = playerPrefab.InstantiateWithInjection();
GameObject enemy = enemyPrefab.InstantiateWithInjection(position, rotation);
GameObject item = itemPrefab.InstantiateWithInjection(parent);

var controller = gameObject.AddComponentWithInjection<PlayerController>();

existingObject.InjectDependencies();
```

### Method 3: Manual Injection

```csharp
GameObject instance = Instantiate(myPrefab);
DIContext.Container.InjectGameObject(instance);
```

## Cascading Dependencies

```csharp
public class ProfileService : IProfileService
{
    private readonly IUserService _userService;
    
    public ProfileService(IUserService userService)
    {
        _userService = userService;
    }
}

public class UserService : IUserService
{
    private readonly IAuthService _authService;
    
    public UserService(IAuthService authService)
    {
        _authService = authService;
    }
}

[Inject]
public IProfileService ProfileService { get; set; }
```

## Cycle Detection

```csharp
public class ServiceA
{
    public ServiceA(ServiceB b) { }
}

public class ServiceB
{
    public ServiceB(ServiceA a) { }
}
```

## Advanced Scopes

```csharp
public class DatabaseManager : MonoBehaviour
{
    [Inject]
    public ILogger Logger { get; set; }

    public void PerformTransaction()
    {
        using (var scope = DIContext.Instance.CreateScope())
        {
            var transaction = scope.Resolve<IDatabaseTransaction>();
            var validator = scope.Resolve<IDataValidator>();
            
            transaction.Begin();
            
            try
            {
                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
            }
        }
    }
}
```

## Best Practices

### 1. Use Interfaces

```csharp
container.Register<ILogger, ConsoleLogger>(Lifetime.Singleton);
```

### 2. Prefer Constructor Injection

```csharp
public class GameManager
{
    private readonly ILogger _logger;
    
    public GameManager(ILogger logger)
    {
        _logger = logger;
    }
}
```

### 3. Use Singleton for Stateless Services

```csharp
container.Register<IEventBus, EventBus>(Lifetime.Singleton);
```

### 4. Use Transient for Stateful Objects

```csharp
container.Register<ICommand, AttackCommand>(Lifetime.Transient);
```

### 5. Use Scoped for Transactions

```csharp
container.Register<IDatabaseTransaction, Transaction>(Lifetime.Scoped);
```

## Manual Resolution

```csharp
var logger = DIContext.Container.Resolve<ILogger>();

var dataService = await DIContext.Container.ResolveAsync<IDataService>();
```

## DIContext Configuration

```csharp
public class MyGameContext : DIContext
{
    [SerializeField] private bool _dontDestroyOnLoad = true;
    [SerializeField] private bool _injectOnAwake = true;

    protected override void ConfigureServices(DIContainer container)
    {
        container.Register<ILogger, ConsoleLogger>(Lifetime.Singleton);
    }
}
```

## Complete Example

See `Examples/ExampleUsage.cs` for full examples including:
- Simple services and services with dependencies  
- Asynchronous injection  
- Scopes  
- Runtime instantiation  
- Cascading dependencies  
- Transactions  

## API Reference

### DIContainer

| Method | Description |
|--------|-------------|
| `Register<TInterface, TImpl>(Lifetime)` | Registers a service |
| `RegisterInstance<T>(instance)` | Registers an existing instance |
| `RegisterFactory<T>(factory, Lifetime)` | Registers with a factory |
| `Resolve<T>()` | Resolves a service |
| `ResolveAsync<T>()` | Resolves with async injection |
| `CreateScope()` | Creates a new scope |
| `InjectGameObject(GameObject)` | Injects into a GameObject |
| `InjectComponent(MonoBehaviour)` | Injects into a component |

### Attributes

| Attribute | Usage |
|-----------|--------|
| `[Inject]` | Property or constructor to inject |
| `[InjectAsync]` | Async initialization method |

### Unity Extensions

| Method | Description |
|--------|-------------|
| `InstantiateWithInjection()` | Instantiates with automatic injection |
| `AddComponentWithInjection<T>()` | Adds a component with injection |
| `InjectDependencies()` | Injects into an existing object |

## Troubleshooting

### Service not registered

Make sure you registered the service in `ConfigureServices()`.

### Circular dependency detected

Review your architecture and use events or the Mediator pattern.

### DIContext.Container is null

Make sure DIContext is in the scene and initialized before use.

### Dependencies are not injected

- Make sure the `[Inject]` attribute is present  
- For runtime GameObjects, use `AutoInject` or the extensions  
- Ensure `InjectOnAwake` is enabled in DIContext  
