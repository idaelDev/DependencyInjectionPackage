# Test 3 : Async Injection

## Objectif
Tester l'injection asynchrone avec [InjectAsync].

## Ce qui est testé
- ✅ Méthodes [InjectAsync] sont appelées
- ✅ Chargement de données async
- ✅ Gestion d'erreurs async

## Setup
1. Scène "03_AsyncInjection"
2. GameObject "DIContext" avec `AsyncInjectionContext`
3. GameObject "Test" avec `AsyncInjectionTest`

## Résultat Attendu
Voir "Loading..." puis "Data loaded: Sample async data"
