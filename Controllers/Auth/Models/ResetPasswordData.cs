namespace chat_server.Controllers.Auth.Models;

public sealed record ResetPasswordData(string Username, string CurrentPassword, string NewPassword);