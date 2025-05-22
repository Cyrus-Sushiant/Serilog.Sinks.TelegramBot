namespace Serilog.Sinks.TelegramBot;

public sealed class TelegramMessage
{
    public TelegramMessage(string text, bool disableNotification = false)
    {
        Text = text;
        DisableNotification = disableNotification;
    }

    public string Text { get; }
    public bool DisableNotification { get; }
}