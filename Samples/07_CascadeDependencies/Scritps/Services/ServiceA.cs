namespace DependencyInjection.Examples._07_CascadeDependencies
{
    public class ServiceA : IServiceA
    {
        private readonly IServiceB _serviceB;
        private readonly ILogger _logger;

        public ServiceA(IServiceB serviceB, ILogger logger)
        {
            _serviceB = serviceB;
            _logger = logger;
            _logger.Log("ServiceA created (depends on IServiceB, ILogger)");
        }

        public string GetData() => _serviceB.GetInfo() + " → A";
    }
}
