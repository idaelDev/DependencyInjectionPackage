namespace DependencyInjection.Samples._05_Scopes
{
    public class TransactionService : ITransactionService
    {
        private readonly ILogger _logger;
        private bool _isActive;

        public TransactionService(ILogger logger)
        {
            _logger = logger;
            _logger.Log("Transaction created");
        }

        public bool IsActive => _isActive;

        public void BeginTransaction()
        {
            _isActive = true;
            _logger.Log("Transaction started");
        }

        public void Commit()
        {
            _isActive = false;
            _logger.Log("Transaction committed");
        }

        public void Dispose()
        {
            if (_isActive)
            {
                _logger.Log("Transaction disposed (rollback)");
            }
            else
            {
                _logger.Log("Transaction disposed");
            }
        }
    }
}
