using UnityEngine;

namespace IdaelDev.DependencyInjection.Samples._01_BasicInjection
{
    public class ConsoleLogger : ILogger
    {
        private readonly string _prefix;

        [Inject]
        public ConsoleLogger() : this("[BasicTest]")
        {
        }

        private ConsoleLogger(string prefix)
        {
            _prefix = prefix;
            Log("Logger initialized");
        }

        public void Log(string message)
        {
            Debug.Log($"{_prefix} {message}");
        }

        public void LogWarning(string message)
        {
            Debug.LogWarning($"{_prefix} {message}");
        }

        public void LogError(string message)
        {
            Debug.LogError($"{_prefix} {message}");
        }
    }
}
