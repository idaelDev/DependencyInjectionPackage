using IdaelDev.DependencyInjection;
using UnityEngine;

namespace DependencyInjection.Samples._04_Lifetimes
{
    public class ConsoleLogger : ILogger
    {
        [Inject] public ConsoleLogger() { }
        public void Log(string message) => Debug.Log($"[LifetimesTest] {message}");
    }
}
