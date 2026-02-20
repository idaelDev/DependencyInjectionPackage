# Test 4 : Lifetimes

## Objectif
Tester les 3 cycles de vie : Singleton, Transient, Scoped.

## Ce qui est testé
- ✅ Singleton retourne la même instance
- ✅ Transient retourne des instances différentes
- ✅ Scoped retourne même instance dans le scope

## Setup
1. Scène "04_Lifetimes"
2. GameObject "DIContext" avec `LifetimesContext`
3. GameObject "Test" avec `LifetimesTest`
