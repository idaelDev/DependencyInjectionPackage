namespace DependencyInjection.Examples._04_Lifetimes
{
    public class ScopedService : IScopedService
    {
        private static int _counter = 0;
        private readonly int _id;
        public ScopedService() { _id = ++_counter; }
        public int GetId() => _id;
    }
}
