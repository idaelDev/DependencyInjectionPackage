# 🚀 Guide de Démarrage Rapide - DI System pour Unity

## Installation en 3 minutes

### Étape 1: Copier les fichiers
1. Copiez le dossier `DependencyInjection` dans votre projet Unity (généralement dans `Assets/Scripts/`)
2. Unity va compiler automatiquement

### Étape 2: Créer votre contexte

Créez un nouveau script C# dans votre projet :

```csharp
using DependencyInjection;
using UnityEngine;

public class MyGameContext : DIContext
{
    protected override void ConfigureServices(DIContainer container)
    {
        // Exemple: Enregistrer un logger singleton
        container.Register<IMyLogger, MyLogger>(Lifetime.Singleton);
        
        // Exemple: Enregistrer un game manager
        container.Register<IGameManager, GameManager>(Lifetime.Singleton);
    }
}

// Interface simple
public interface IMyLogger
{
    void Log(string message);
}

// Implémentation
public class MyLogger : IMyLogger
{
    public void Log(string message)
    {
        Debug.Log($"[Game] {message}");
    }
}
```

### Étape 3: Ajouter à la scène

1. Dans Unity, créez un nouveau GameObject vide (clic droit → Create Empty)
2. Renommez-le "DIContext"
3. Ajoutez votre script `MyGameContext` dessus
4. Dans l'inspecteur, cochez "Dont Destroy On Load" et "Inject On Awake"

### Étape 4: Utiliser l'injection

Dans n'importe quel MonoBehaviour :

```csharp
using DependencyInjection;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Inject]
    public IMyLogger Logger { get; set; }
    
    private void Start()
    {
        Logger.Log("Player initialized!");
    }
}
```

C'est tout ! 🎉

## Premier Test

1. Créez un GameObject avec le script `PlayerController`
2. Lancez la scène
3. Vous devriez voir "[Game] Player initialized!" dans la console

## Prochaines Étapes

### Pour les prefabs instanciés au runtime :

**Option A - Automatique (Recommandé)**
1. Sur votre prefab, ajoutez le composant `AutoInject`
2. Cochez "Inject On Awake"
3. Utilisez `Instantiate()` normalement - l'injection se fait automatiquement

**Option B - Extensions**
```csharp
// Utilisez InstantiateWithInjection au lieu de Instantiate
GameObject instance = myPrefab.InstantiateWithInjection();
```

### Pour des services avec dépendances :

```csharp
// Service qui dépend d'un autre
public class GameManager : IGameManager
{
    private readonly IMyLogger _logger;
    
    [Inject]
    public GameManager(IMyLogger logger)
    {
        _logger = logger;
    }
    
    public void StartGame()
    {
        _logger.Log("Game started!");
    }
}

// Dans MyGameContext
protected override void ConfigureServices(DIContainer container)
{
    container.Register<IMyLogger, MyLogger>(Lifetime.Singleton);
    container.Register<IGameManager, GameManager>(Lifetime.Singleton);
    // GameManager recevra automatiquement le logger !
}
```

### Pour l'injection asynchrone :

```csharp
public class DataLoader : MonoBehaviour
{
    [Inject]
    public IMyLogger Logger { get; set; }
    
    // Cette méthode sera appelée automatiquement après l'injection
    [InjectAsync]
    private async Task LoadDataAsync(IDataService dataService)
    {
        Logger.Log("Loading...");
        await dataService.LoadAsync();
        Logger.Log("Done!");
    }
}
```

## Erreurs Courantes

### ❌ "Service not registered"
**Solution:** Ajoutez `container.Register<...>()` dans `ConfigureServices()`

### ❌ "DIContext.Container is null"
**Solution:** Assurez-vous que le GameObject DIContext existe dans la scène et est activé

### ❌ Les propriétés [Inject] restent null
**Solutions:**
- Vérifiez que DIContext est dans la scène
- Pour les objets instanciés au runtime, ajoutez `AutoInject` ou utilisez les extensions
- Vérifiez que "Inject On Awake" est coché dans DIContext

### ❌ "Circular dependency detected"
**Solution:** Vous avez une boucle dans vos dépendances (A dépend de B qui dépend de A). Revoyez votre architecture.

## 📚 Documentation Complète

Consultez le fichier `README.md` pour :
- Tous les cycles de vie (Singleton, Transient, Scoped)
- Scopes temporaires
- Factories personnalisées
- Exemples avancés
- API complète

## 💡 Conseils

1. **Commencez simple** - Un seul service singleton suffit pour débuter
2. **Testez progressivement** - Ajoutez un service à la fois
3. **Utilisez des interfaces** - Facilite les tests et la maintenance
4. **Pour MonoBehaviour** - Préférez l'injection de propriété avec `[Inject]`
5. **Pour les services purs** - Préférez l'injection par constructeur

Bon développement ! 🎮
