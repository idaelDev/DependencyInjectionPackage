using System.Threading.Tasks;

namespace DependencyInjection.Examples._03_AsyncInjection
{
    public class DataService : IDataService
    {
        private readonly ILogger _logger;

        public DataService(ILogger logger)
        {
            _logger = logger;
        }

        public async Task<string> LoadDataAsync()
        {
            _logger.Log("Loading data...");
            await Task.Delay(1000);
            _logger.Log("Data loaded!");
            return "Sample async data";
        }
    }
}
