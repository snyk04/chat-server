namespace Server.Models;

public abstract class Message
{
    public string Author { get; set; }

    protected Message(string author)
    {
        Author = author;
    }
}