namespace DependencyInjection.Samples._04_Lifetimes
{
    public class ScopedService : IScopedService
    {
        private static int _counter ;
        private readonly int _id;
        public ScopedService() { _id = ++_counter; }
        public int GetId() => _id;
    }
}
