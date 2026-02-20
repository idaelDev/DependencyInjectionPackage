using UnityEngine;

namespace DependencyInjection.Examples._06_RuntimeInstantiation
{
    public class ConsoleLogger : ILogger
    {
        [Inject] public ConsoleLogger() { }
        public void Log(string message) => Debug.Log($"[RuntimeTest] {message}");
    }
}
