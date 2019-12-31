namespace Serilog.Sinks.TelegramBot
{
    public sealed class TelegramMessage
    {
        public TelegramMessage(string text)
        {
            Text = text;
        }

        public string Text { get; }
    }
}