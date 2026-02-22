using IdaelDev.DependencyInjection;
using UnityEngine;

namespace DependencyInjection.Samples._07_CascadeDependencies
{
    public class ConsoleLogger : ILogger
    {
        [Inject] public ConsoleLogger() { }
        public void Log(string message) => Debug.Log($"[CascadeTest] {message}");
    }

}
