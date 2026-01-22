using System;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using UnityEngine;
using VContainer;

public class AuthService : BaseService, IAuthService
{

    [Inject] private IAuthRepository authRepository;

    [UnityEngine.Scripting.Preserve]
    [UsedImplicitly]
    public AuthService() { }


    public async UniTask<(bool Success, RequestErrorInfo ErrorInfo)> LoginAsync(LoginModel loginModel)
    {
        if (Launch.Instance.showDebugBackend) Debug.Log($"{nameof(AuthService)}::{nameof(LoginAsync)} username= {loginModel.Username}");

        var (loginSuccess, dataAsText) = await authRepository.LoginAsync(loginModel);

        if (loginSuccess)
        {
            //ClientPreferences.UserName = username;
        }

        var errorInfo = RequestErrorInfo.LoginFailedError;

        try
        {
            errorInfo = serializationService.Deserialize<RequestErrorInfo>(dataAsText);
        }
        catch (Exception e)
        {
            // ignored
            if (Launch.Instance.showDebugBackend) Debug.LogWarning($"{nameof(AuthService)}::{nameof(LoginAsync)} " +
                                             $"parsing error info failed! {e.Message}"
            );
        }

        return (Success: loginSuccess, ErrorInfo: errorInfo);
    }

    public async UniTask<bool> RefreshToken(string refreshToken)
    {
        if (Launch.Instance.showDebugBackend) Debug.Log($"{nameof(AuthService)}::{nameof(RefreshToken)}");

        var responseSuccess = await authRepository.RefreshToken(refreshToken);

        // if (responseSuccess) {
        //     ClientPreferences.UserName = username;
        // }

        return responseSuccess;
    }

    public async UniTask<ComprehensiveUserData> GetUserDataAsync()
    {
        return await DoRequest<ComprehensiveUserData>(
            request: authRepository.GetUserDataAsync(),
            requestName: nameof(GetUserDataAsync)
        );
    }
}

