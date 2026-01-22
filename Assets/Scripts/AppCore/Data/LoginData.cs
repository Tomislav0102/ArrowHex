using Newtonsoft.Json;
using System;

[Serializable]
public class LoginData
{
    [JsonProperty("username")]
    public string Username { get; set; }

    public LoginData(LoginModel loginModel)
    {
        Username = loginModel.Username;
    }
}
