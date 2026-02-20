namespace DependencyInjection.Samples._04_Lifetimes
{

    public class SingletonService : ISingletonService
    {
        private static int _counter;
        private readonly int _id;
        public SingletonService() { _id = ++_counter; }
        public int GetId() => _id;
    }
}
