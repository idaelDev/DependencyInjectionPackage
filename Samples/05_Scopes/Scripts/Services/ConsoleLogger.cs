using UnityEngine;

namespace DependencyInjection.Samples._05_Scopes
{
    public class ConsoleLogger : ILogger
    {
        [Inject] public ConsoleLogger() { }
        public void Log(string message) => Debug.Log($"[ScopesTest] {message}");
    }
}
