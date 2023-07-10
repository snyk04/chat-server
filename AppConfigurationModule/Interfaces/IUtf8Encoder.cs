namespace chat_server.AppConfigurationModule.Interfaces;

public interface IUtf8Encoder
{
    byte[] GetBytes(string str);
}