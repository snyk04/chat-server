namespace chat_server.Controllers.Auth.Models;

public class ResetPasswordData
{
    public string Username { get; set; }
    public string CurrentPassword { get; set; }
    public string NewPassword { get; set; }
}