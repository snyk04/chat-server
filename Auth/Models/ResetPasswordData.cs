namespace chat_server.Auth.Models;

public class ResetPasswordData
{
    public string Username { get; set; }
    public string CurrentPassword { get; set; }
    public string NewPassword { get; set; }
}