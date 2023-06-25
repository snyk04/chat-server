namespace chat_server.Controllers.Chat.Models;

public abstract class Message
{
    public string Author { get; set; }

    protected Message(string author)
    {
        Author = author;
    }
}