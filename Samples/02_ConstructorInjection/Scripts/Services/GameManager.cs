namespace DependencyInjection.Examples._02_ConstructorInjection
{
    public class GameManager : IGameManager
    {
        private readonly ILogger _logger;
        private readonly IAudioService _audioService;
        private bool _isRunning;

        [Inject]
        public GameManager(ILogger logger, IAudioService audioService)
        {
            _logger = logger;
            _audioService = audioService;
            _logger.Log("GameManager initialized with constructor injection");
        }

        public bool IsRunning => _isRunning;

        public void StartGame()
        {
            _isRunning = true;
            _logger.Log("Game started!");
            _audioService.PlaySound("GameStart");
        }
    }
}
