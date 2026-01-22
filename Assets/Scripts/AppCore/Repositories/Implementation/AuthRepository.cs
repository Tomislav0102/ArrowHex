using Best.HTTP;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using Newtonsoft.Json;
//using Rexee.AppCore.Data;
//using Rexee.AppCore.Repositories.Implementation.Extensions;
//using Rexee.AppCore.Repositories.Interfaces;
using UnityEngine;

public class AuthRepository : RepositoryBase, IAuthRepository
{

    //private const string LoginPayload = "{{\"username\": \"{0}\",\"password\": \"{1}\"}}";

    [UnityEngine.Scripting.Preserve]
    [UsedImplicitly]
    public AuthRepository() { }

    public async UniTask<(bool Success, string DataAsText)> LoginAsync(LoginModel loginModel)
    {
        #region Request
        LoginData data = new LoginData(loginModel);
        string payload = JsonConvert.SerializeObject(data);
        var request = CreatePostRequest(payload, apiConfig.LoginUrl, false, nameof(AuthRepository), nameof(LoginAsync));
        #endregion

        var (success, dataAsText) = await DoRequestInternal(request);
        if (!success)
        {
            if (Launch.Instance.showDebugBackend) Debug.Log($"{nameof(AuthRepository)}::{nameof(LoginAsync)} failed dataAsText= {dataAsText}");
            return (Success: false, DataAsText: dataAsText);
        }

        var deserialized = JsonConvert.DeserializeObject<ApiAccessToken>(dataAsText);
        accessToken.ApplyFrom(deserialized);

        // TODO: set token to LocalUserData
        //ClientPreferences.AccessToken = accessToken.Token;
        //ClientPreferences.RefreshToken = accessToken.RefreshToken;
        //ClientPreferences.TokenExpirationDate = accessToken.ExpirationDate;
        LocalUserData.Token = accessToken;
        
        // todo: temporary while HttpsClient is used
        PlayerPrefs.SetString("JWT", accessToken.Token);

        if (Launch.Instance.showDebugBackend) Debug.Log($"{nameof(AuthRepository)}::{nameof(LoginAsync)} token= {accessToken.Token} " +
                                                        $"refreshToken= {accessToken.RefreshToken}");
        return (Success: true, DataAsText: dataAsText);
    }
    
    public async UniTask<(bool Success, string DataAsText)> GetUserDataAsync()
    {
        string repositoryName = nameof(AuthRepository);
        string methodName = nameof(GetUserDataAsync);

        #region Request
        var request = CreateGetRequest(apiConfig.UserDataUrl, repositoryName, methodName);
        #endregion

        var (success, dataAsText) = await DoRequest(request);
        if (!success)
        {
            if (Launch.Instance.showDebugBackend) Debug.Log($"{repositoryName}::{methodName} failed dataAsText= {dataAsText}");
        }
        return (Success: success, DataAsText: dataAsText);
    }
}

