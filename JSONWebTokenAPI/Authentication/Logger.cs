namespace JSONWebTokenAPI.Authentication
{
    public class Logger<T> : ILogger<T>
    {
        private readonly Microsoft.Extensions.Logging.ILogger<T> _logger;
        public Logger(Microsoft.Extensions.Logging.ILogger<T> logger)
        {
            _logger = logger;
        }

        public void LogInformation(string message)
        {
            _logger.LogInformation(message);
        }

        public void LogError(string message)
        {
            _logger.LogError(message);
        }

        public void LogWarning(string message)
        {
            _logger.LogWarning(message);
        }
    }
}
