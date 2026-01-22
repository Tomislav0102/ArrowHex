using System;
using System.Text;
using Newtonsoft.Json;
using Sirenix.Utilities;

[Serializable]
public class RequestErrorInfo
{
    private const string GeneralErrorMessage = "Request Error";
    private const string LoginFailedErrorMessage = "Login Failed (Unknown Reason)";

    public static readonly RequestErrorInfo GeneralError = new(
        statusCode: -1,
        dataAsText: GeneralErrorMessage
    );

    public static readonly RequestErrorInfo LoginFailedError = new RequestErrorInfo(
        statusCode: -1,
        dataAsText: LoginFailedErrorMessage
    );

    public int StatusCode { get; private set; } = -1;
    public string DataAsText { get; private set; } = GeneralErrorMessage;
    public string ErrorMessage { get; private set; } = string.Empty;

    [JsonIgnore]
    public string DisplayMessage => $"Status: ({StatusCode}) {DataAsText}";

    public RequestErrorInfo() { }

    [JsonConstructor]
    public RequestErrorInfo(int statusCode, string dataAsText, string errorMessage = "")
    {
        StatusCode = statusCode;
        DataAsText = dataAsText;
        ErrorMessage = errorMessage;

        if (dataAsText.IsNullOrWhitespace())
        {
            DataAsText = GeneralErrorMessage;
        }
    }

    public override string ToString()
    {
        var sb = new StringBuilder();

        sb.Append($"{nameof(StatusCode)}: {StatusCode} ");
        sb.Append($"{nameof(DataAsText)}: {DataAsText} ");
        sb.Append($"{nameof(ErrorMessage)}: {ErrorMessage} ");
        sb.Append($"{nameof(DisplayMessage)}: {DisplayMessage}");

        return sb.ToString();
    }
}
