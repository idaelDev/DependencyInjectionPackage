# Test 1 : Basic Injection

## Objectif
Tester l'injection de base sur les fields et properties dans un MonoBehaviour.

## Ce qui est testé
- ✅ Injection de field privé avec `[Inject]`
- ✅ Injection de property publique avec `[Inject]`
- ✅ Résolution de service singleton

## Setup dans Unity

### 1. Créer une nouvelle scène
- File → New Scene
- Sauvegardez-la comme "01_BasicInjection"

### 2. Ajouter le DIContext
- GameObject → Create Empty
- Renommez-le "DIContext"
- Ajoutez le composant `BasicInjectionContext`
- Cochez "Inject On Awake"

### 3. Ajouter le Test
- GameObject → Create Empty
- Renommez-le "Test"
- Ajoutez le composant `BasicInjectionTest`

### 4. Lancer
- Appuyez sur Play
- Vérifiez la console et l'UI

## Résultat Attendu

Console:
```
[BasicTest] Logger initialized
[BasicTest] ✓ Field injection works!
[BasicTest] ✓ Property injection works!

=== Basic Injection Test Results ===
Field Injection: PASS ✓
Property Injection: PASS ✓
====================================
```

UI:
```
=== Basic Injection Test ===
Field Injection: ✓ PASS
Property Injection: ✓ PASS
```

## Fichiers Inclus
- `ILogger.cs` - Interface du logger
- `ConsoleLogger.cs` - Implémentation
- `BasicInjectionTest.cs` - Composant de test
- `BasicInjectionContext.cs` - Configuration DI
- `README.md` - Ce fichier
