# Test 2 : Constructor Injection

## Objectif
Tester l'injection par constructeur pour les classes normales (non-MonoBehaviour).

## Ce qui est testé
- ✅ Injection par constructeur avec `[Inject]`
- ✅ Résolution de dépendances en cascade
- ✅ Multiple dépendances dans un constructeur

## Setup
1. Nouvelle scène "02_ConstructorInjection"
2. GameObject "DIContext" avec `ConstructorInjectionContext`
3. GameObject "Test" avec `ConstructorInjectionTest`
4. Play

## Résultat Attendu
```
[ConstructorTest] Logger initialized
[ConstructorTest] AudioService initialized
[ConstructorTest] GameManager initialized with constructor injection
[ConstructorTest] Game started!
[ConstructorTest] Playing sound: GameStart
```
