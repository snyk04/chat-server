namespace chat_server.Controllers.Chat.Models;

public record TextMessage(string Text, string Author) : Message(Author);