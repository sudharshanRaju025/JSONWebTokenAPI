using System.Text.RegularExpressions;

namespace JSONWebTokenAPI
{
    public interface ICredentialsValidator
    {
        bool IsValidUsername(string username);
        bool IsValidPassword(string password);
        bool IsValidEmail(string Email);
    }

    public class CredentialsValidator : ICredentialsValidator
    {
        private static readonly Regex UsernameRegex =
            new Regex(@"^[A-Z][a-z0-9\W]*$");

        private static readonly Regex PasswordRegex =
            new Regex(@"^(?=^[A-Z])(?=.*\d)(?=.*\W).+$");

        private static readonly Regex EmailRegex =
            new Regex(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$^");
 
        public bool IsValidUsername(string username) =>
            !string.IsNullOrEmpty(username) && UsernameRegex.IsMatch(username);

        public bool IsValidPassword(string password) =>
            !string.IsNullOrEmpty(password) && PasswordRegex.IsMatch(password);

        public bool IsValidEmail(string Email) =>
            !string.IsNullOrEmpty(Email) && EmailRegex.IsMatch(Email);
    }
}
