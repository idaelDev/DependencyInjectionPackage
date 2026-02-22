namespace IdaelDev.DependencyInjection.Samples._02_ConstructorInjection
{
    public class AudioService : IAudioService
    {
        private readonly ILogger _logger;

        public AudioService(ILogger logger)
        {
            _logger = logger;
            _logger.Log("AudioService initialized");
        }

        public void PlaySound(string soundName)
        {
            _logger.Log($"Playing sound: {soundName}");
        }
    }
}
