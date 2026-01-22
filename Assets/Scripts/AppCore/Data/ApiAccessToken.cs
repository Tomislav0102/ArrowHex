using System;
using System.Globalization;
using Newtonsoft.Json;
using UnityEngine;


[Serializable]
public class ApiAccessToken
{

    [JsonProperty("access")]
    public string Token { get; set; }

    [JsonProperty("refresh")]
    public string RefreshToken { get; set; }

    [JsonProperty("expires_in")]
    public int ExpiresIn { get; set; }

    [JsonIgnore]
    public DateTime ExpirationDate { get; set; } = DateTime.Now;

    [JsonIgnore]
    public bool HasExpired => ExpirationDate < DateTime.Now;
    
    public void ApplyFrom(ApiAccessToken deserialized)
    {
        if (deserialized == null)
        {
            if (Launch.Instance.showDebugBackend) Debug.LogWarning($"{nameof(ApiAccessToken)}::{nameof(ApplyFrom)} cannot apply null token!");

            return;
        }

        Token = deserialized.Token;
        RefreshToken = deserialized.RefreshToken;
        //ExpiresIn = deserialized.ExpiresIn;
        ExpiresIn = DecodeAccessToken(Token).exp;
        if (Launch.Instance.showDebugBackend) Debug.Log($"ExpiresIn: {ExpiresIn}");

        // todo: for testing change to 1 minute
        ExpirationDate = DateTime.Now.AddSeconds(ExpiresIn);

        if (Launch.Instance.showDebugBackend) Debug.Log($"{nameof(ApiAccessToken)}::{nameof(ApplyFrom)} accessToken = {Token}, " +
                                  $"refreshToken = {RefreshToken}, " +
                                  $"expirationDate = {ExpirationDate.ToString(CultureInfo.CurrentCulture)}");
    }

    public void ApplyFrom(string accessToken, string refreshToken, DateTime expirationDate)
    {

        Token = accessToken;
        RefreshToken = refreshToken;
        // todo: for testing change to 1 minute
        ExpirationDate = expirationDate;
        ExpiresIn = (int)(expirationDate - DateTime.Now).TotalSeconds;

        if (Launch.Instance.showDebugBackend) Debug.Log($"{nameof(ApiAccessToken)}::{nameof(ApplyFrom)} accessToken = {Token}, " +
                                  $"refreshToken = {RefreshToken}, " +
                                  $"expirationDate = {ExpirationDate.ToString(CultureInfo.CurrentCulture)}");
    }

    public bool IsTokenValid()
    {
        return ExpirationDate > DateTime.Now;
    }

    public void ClearLogin()
    {
        Token = string.Empty;
        RefreshToken = string.Empty;
        ExpiresIn = 0;
        ExpirationDate = DateTime.MinValue;
    }

    private DecodedToken DecodeAccessToken(string token)
    {
        var parts = token.Split('.');
        string userInfo = "";
        if (parts.Length > 2)
        {
            var decode = parts[1];
            var padLength = 4 - decode.Length % 4;
            if (padLength < 4)
            {
                decode += new string('=', padLength);
            }
            var bytes = System.Convert.FromBase64String(decode);
            userInfo = System.Text.ASCIIEncoding.ASCII.GetString(bytes);
        }
        return JsonConvert.DeserializeObject<DecodedToken>(userInfo);
    }
}

