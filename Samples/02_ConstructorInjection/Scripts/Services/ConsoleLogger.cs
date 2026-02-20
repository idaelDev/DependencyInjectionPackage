using UnityEngine;

namespace DependencyInjection.Examples._02_ConstructorInjection
{
    public class ConsoleLogger : ILogger
    {
        [Inject]
        public ConsoleLogger()
        {
            Log("Logger initialized");
        }

        public void Log(string message)
        {
            Debug.Log($"[ConstructorTest] {message}");
        }
    }
}
