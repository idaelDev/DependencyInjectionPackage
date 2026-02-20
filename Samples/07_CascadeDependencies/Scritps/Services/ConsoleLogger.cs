using UnityEngine;

namespace DependencyInjection.Examples._07_CascadeDependencies
{
    public class ConsoleLogger : ILogger
    {
        [Inject] public ConsoleLogger() { }
        public void Log(string message) => Debug.Log($"[CascadeTest] {message}");
    }

}
