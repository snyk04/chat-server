namespace chat_server.Controllers.Chat.Interfaces;

public delegate void ReceiveMessageHandler(string messageText, string username);