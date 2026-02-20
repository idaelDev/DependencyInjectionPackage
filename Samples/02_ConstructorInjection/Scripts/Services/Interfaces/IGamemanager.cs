namespace DependencyInjection.Samples._02_ConstructorInjection
{
    public interface IGameManager
    {
        void StartGame();
        bool IsRunning { get; }
    }
}
