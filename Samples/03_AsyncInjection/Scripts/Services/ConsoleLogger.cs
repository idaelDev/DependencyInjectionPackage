using UnityEngine;

namespace DependencyInjection.Samples._03_AsyncInjection
{
    public class ConsoleLogger : ILogger
    {
        [Inject]
        public ConsoleLogger() { }

        public void Log(string message)
        {
            Debug.Log($"[AsyncTest] {message}");
        }
    }

}
