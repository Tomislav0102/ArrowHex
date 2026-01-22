using Cysharp.Threading.Tasks;

/// <summary>
/// Interface for authentication with backend.
/// </summary>
public interface IAuthService
{
    UniTask<bool> RefreshToken(string refreshToken);
    UniTask<(bool Success, RequestErrorInfo ErrorInfo)> LoginAsync(LoginModel loginModel);
    UniTask<ComprehensiveUserData> GetUserDataAsync();
}

