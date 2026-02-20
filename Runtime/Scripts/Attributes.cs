using System;

namespace DependencyInjection
{
    /// <summary>
    /// Marque un field, une propriété, un constructeur ou un paramètre pour l'injection de dépendances
    /// </summary>
    [AttributeUsage(
        AttributeTargets.Field | 
        AttributeTargets.Property | 
        AttributeTargets.Constructor | 
        AttributeTargets.Parameter, 
        AllowMultiple = false)]
    public class InjectAttribute : Attribute
    {
    }

    /// <summary>
    /// Marque une méthode pour l'injection asynchrone de dépendances
    /// La méthode sera appelée après la construction de l'objet avec les dépendances injectées
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class InjectAsyncAttribute : Attribute
    {
    }
}
