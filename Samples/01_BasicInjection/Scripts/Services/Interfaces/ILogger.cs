namespace IdaelDev.DependencyInjection.Samples._01_BasicInjection
{
    public interface ILogger
    {
        void Log(string message);
        void LogWarning(string message);
        void LogError(string message);
    }
}
