using System;
using System.IO;
using System.Text;
using Best.HTTP;
using Best.HTTP.Request.Authenticators;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using Rexee.AppCore.Repositories.Implementation.Extensions;

//using Rexee.AppCore.Config.Interfaces;
//using Rexee.AppCore.Data;
//using Rexee.AppCore.Repositories.Implementation.Extensions;
using UnityEngine;
using VContainer;

/// <summary>
/// Base class for repository implementations.
/// </summary>
public abstract class RepositoryBase
{

    [Inject] protected IApiConfig apiConfig;
    [Inject] protected ApiAccessToken accessToken;

    private const string RefreshPayload = "{{\"refresh\": \"{0}\"}}";
    

    protected void Authorize(HTTPRequest request)
    {
        if (Launch.Instance.showDebugBackend) Debug.Log($"{nameof(RepositoryBase)}::{nameof(Authorize)} token = {accessToken.Token}");
        request.Authenticator = new BearerTokenAuthenticator(accessToken.Token);
    }

    protected async UniTask<(bool Success, string DataAsText)> DoRequest(HTTPRequest request)
    {
        try
        {
            return await DoRequestInternal(request);

            //if (!accessToken.HasExpired)
            //{
            //    return await DoRequestInternal(request);
            //}

            //#region Access token expired
            //var success = await RefreshToken(accessToken.RefreshToken);

            //if (success)
            //{
            //    Authorize(request);
            //}
            //else
            //{
            //    Debug.LogError($"{nameof(RepositoryBase)}::{nameof(DoRequest)} Unable to refresh token!");

            //    return (Success: false, DataAsText: string.Empty);
            //}

            //// do request after token refresh
            //return await DoRequestInternal(request);
            //#endregion
        }
        catch (AsyncHTTPException e)
        {
            if (Launch.Instance.showDebugBackend) Debug.LogError($"{nameof(RepositoryBase)}::{nameof(DoRequest)} Error {e.Message}");
            if (Launch.Instance.showDebugBackend) Debug.LogException(e);
        }


        return (Success: false, DataAsText: string.Empty);
    }

    protected static async UniTask<(bool Success, string DataAsText)> DoRequestInternal(HTTPRequest request)
    {
        var response = await request.GetHTTPResponseAsync();

        #region Success
        if (response.IsSuccess)
        {
            if (Launch.Instance.showDebugBackend) Debug.Log($"{nameof(RepositoryBase)}::{nameof(DoRequest)} " +
                                      $"request completed status= {response.StatusCode} " +
                                      $"responseText= {response.DataAsText}"
            );

            return (Success: true, response.DataAsText);
        }
        #endregion

        #region Failed
        Debug.LogWarning($"{nameof(RepositoryBase)}::{nameof(DoRequest)} " +
                         $"request FAILED status= {response.StatusCode} " +
                         $"responseText= {response.DataAsText} " +
                         $"message= {response.Message}"
        );

        var requestErrorInfo = new RequestErrorInfo(
            statusCode: response.StatusCode,
            dataAsText: response.DataAsText,
            errorMessage: response.Message
        );

        var serializedError = JsonConvert.SerializeObject(requestErrorInfo, Formatting.Indented);
        return (Success: false, DataAsText: serializedError);
        #endregion
    }

    public async UniTask<bool> RefreshToken(string refreshToken)
    {
        if (Launch.Instance.showDebugBackend) Debug.Log($"{nameof(RepositoryBase)}::{nameof(RefreshToken)} refreshToken= {refreshToken} loginUrl= {apiConfig.RefreshUrl}");

        var payload = string.Format(RefreshPayload, refreshToken);
        var bytes = Encoding.UTF8.GetBytes(payload);

        var request = HTTPRequest.CreatePost(apiConfig.RefreshUrl);
        request.SetJsonContentTypeHeader();
        request.UploadSettings.UploadStream = new MemoryStream(bytes);

        var (success, dataAsText) = await DoRequestInternal(request);

        if (!success)
        {
            if (Launch.Instance.showDebugBackend) Debug.Log($"{nameof(RepositoryBase)}::{nameof(RefreshToken)} failed dataAsText= {dataAsText}");
            return false;
        }

        var deserialized = JsonConvert.DeserializeObject<ApiAccessToken>(dataAsText);
        accessToken.ApplyFrom(deserialized);

        // TODO: repo should not set these
        //ClientPreferences.AccessToken = accessToken.Token;
        //ClientPreferences.RefreshToken = accessToken.RefreshToken;
        //ClientPreferences.TokenExpirationDate = accessToken.ExpirationDate;
        LocalUserData.Token = deserialized;

        // HttpsClient
        PlayerPrefs.SetString("JWT", accessToken.Token);

        if (Launch.Instance.showDebugBackend) Debug.Log($"{nameof(RepositoryBase)}::{nameof(RefreshToken)} token= {accessToken.Token}");

        return true;
    }

    protected HTTPRequest CreatePostRequest(string payload, string url, bool isAuthorized, string className, string methodName)
    {
        var bytes = Encoding.UTF8.GetBytes(payload);
        var request = HTTPRequest.CreatePost(url);
        request.SetJsonContentTypeHeader();
        request.UploadSettings.UploadStream = new MemoryStream(bytes);
        if (isAuthorized)
            Authorize(request);

        if (Launch.Instance.showDebugBackend) Debug.Log($"{className}::{methodName} payload= {payload}");
        if (Launch.Instance.showDebugBackend) Debug.Log($"Sending {methodName} to: {url}");
        return request;
    }

    protected HTTPRequest CreatePutRequest(string payload, string url, bool isAuthorized, string className, string methodName)
    {
        var bytes = Encoding.UTF8.GetBytes(payload);
        var request = HTTPRequest.CreatePut(url);
        request.SetJsonContentTypeHeader();
        request.UploadSettings.UploadStream = new MemoryStream(bytes);
        if (isAuthorized)
            Authorize(request);

        if (Launch.Instance.showDebugBackend) Debug.Log($"{className}::{methodName} payload= {payload}");
        if (Launch.Instance.showDebugBackend) Debug.Log($"Sending {methodName} to: {url}");
        return request;
    }

    protected HTTPRequest CreateGetRequest(string url, string className, string methodName)
    {
        var request = HTTPRequest.CreateGet(url);
        request.SetJsonContentTypeHeader();
        Authorize(request);

        if (Launch.Instance.showDebugBackend) Debug.Log($"{className}::{methodName} GET");
        if (Launch.Instance.showDebugBackend) Debug.Log($"Sending {methodName} to: {url}");
        return request;
    }
}

