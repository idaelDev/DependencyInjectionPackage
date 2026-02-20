namespace DependencyInjection.Samples._05_Scopes
{
    public interface ITransactionService : System.IDisposable
    {
        void BeginTransaction();
        void Commit();
        bool IsActive { get; }
    }
}
