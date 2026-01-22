public class LoginModel
{
    public string Username { get; set; }
    public bool IsError { get; set; }
    public string ErrorMessage { get; set; }

    public LoginModel (string email)
    {
        Username = email;
    }

    public void SetError(string errorMessage)
    {
        IsError = true;
        ErrorMessage = errorMessage;
    }

    public void ClearError()
    {
        IsError = false;
        ErrorMessage = string.Empty;
    }
}
