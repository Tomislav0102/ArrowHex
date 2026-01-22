using Cysharp.Threading.Tasks;

public interface IAuthRepository
{
    UniTask<bool> RefreshToken(string refreshToken);
    UniTask<(bool Success, string DataAsText)> LoginAsync(LoginModel loginModel);
    UniTask<(bool Success, string DataAsText)> GetUserDataAsync();
}
