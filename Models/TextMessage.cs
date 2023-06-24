namespace Server.Models;

public class TextMessage : Message
{
    public string Text { get; set; }
    
    public TextMessage(string text, string author) : base(author)
    {
        Text = text;
    }
}