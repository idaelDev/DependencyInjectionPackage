namespace DependencyInjection.Samples._03_AsyncInjection
{
    public interface IDataService
    {
        System.Threading.Tasks.Task<string> LoadDataAsync();
    }
}
