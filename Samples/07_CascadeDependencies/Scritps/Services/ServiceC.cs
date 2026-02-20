namespace DependencyInjection.Samples._07_CascadeDependencies
{
    public class ServiceC : IServiceC
    {
        private readonly ILogger _logger;

        public ServiceC(ILogger logger)
        {
            _logger = logger;
            _logger.Log("ServiceC created (depends on ILogger)");
        }

        public string GetResult() => "Data from C";
    }
}
