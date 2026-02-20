namespace DependencyInjection.Samples._07_CascadeDependencies
{
    public class ServiceB : IServiceB
    {
        private readonly IServiceC _serviceC;
        private readonly ILogger _logger;

        public ServiceB(IServiceC serviceC, ILogger logger)
        {
            _serviceC = serviceC;
            _logger = logger;
            _logger.Log("ServiceB created (depends on IServiceC, ILogger)");
        }

        public string GetInfo() => _serviceC.GetResult() + " → B";
    }
}
