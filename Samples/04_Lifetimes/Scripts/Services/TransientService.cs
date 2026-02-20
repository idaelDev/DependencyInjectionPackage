namespace DependencyInjection.Examples._04_Lifetimes
{
    public class TransientService : ITransientService
    {
        private static int _counter = 0;
        private readonly int _id;
        public TransientService() { _id = ++_counter; }
        public int GetId() => _id;
    }
}
