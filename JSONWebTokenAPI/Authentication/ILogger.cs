namespace JSONWebTokenAPI.Authentication
{
    public interface ILogger<T>
    {
        void LogInformation(string Message);

        void LogError(string Message);

        void LogWarning(string message);
    }
}

