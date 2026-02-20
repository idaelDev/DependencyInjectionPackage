namespace DependencyInjection.Examples._02_ConstructorInjection
{
    public interface IGameManager
    {
        void StartGame();
        bool IsRunning { get; }
    }
}
