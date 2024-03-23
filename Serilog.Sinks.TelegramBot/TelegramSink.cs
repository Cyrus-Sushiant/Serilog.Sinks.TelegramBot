using Serilog.Core;
using Serilog.Debugging;
using Serilog.Events;
using System;
using System.Text;

namespace Serilog.Sinks.TelegramBot;

public class TelegramSink : ILogEventSink
{
    /// <summary>
    /// Delegate to allow overriding of the RenderMessage method.
    /// </summary>
    public delegate TelegramMessage RenderMessageMethod(LogEvent input);

    protected static string ApplicationName;
    protected static ParseMode ParseMode;
    private readonly string _chatId;
    private readonly string _token;
    protected readonly IFormatProvider FormatProvider;

    /// <summary>
    /// RenderMessage method that will transform LogEvent into a Telegram message.
    /// </summary>
    protected RenderMessageMethod RenderMessageImplementation = RenderMessage;

    public TelegramSink(string chatId, string token, string applicationName, ParseMode parseMode, RenderMessageMethod renderMessageImplementation, IFormatProvider formatProvider)
    {
        if (string.IsNullOrWhiteSpace(value: chatId))
            throw new ArgumentNullException(paramName: nameof(chatId));

        if (string.IsNullOrWhiteSpace(value: token))
            throw new ArgumentNullException(paramName: nameof(token));

        _chatId = chatId;
        _token = token;
        ApplicationName = applicationName;
        ParseMode = parseMode;
        FormatProvider = formatProvider;

        if (renderMessageImplementation is not null)
            RenderMessageImplementation = renderMessageImplementation;
    }

    #region ILogEventSink implementation

    public void Emit(LogEvent logEvent)
    {
        var message = FormatProvider != null
            ? new TelegramMessage(text: logEvent.RenderMessage(formatProvider: FormatProvider))
            : RenderMessageImplementation(input: logEvent);

        SendMessage(token: _token, chatId: _chatId, message: message);
    }

    #endregion

    protected static TelegramMessage RenderMessage(LogEvent logEvent)
    {
        var sb = new StringBuilder();
        if (ParseMode == ParseMode.Markdown)
        {
            sb.AppendLine(value: $"{GetEmoji(log: logEvent)} {logEvent.RenderMessage()}");
        }
        else
        {
            sb.AppendLine(value: $"{GetEmoji(log: logEvent)} {logEvent.RenderMessage().HtmlEncode()}");
        }

        if (!string.IsNullOrEmpty(ApplicationName))
        {
            sb.AppendLine(
                value: ParseMode == ParseMode.Markdown
                    ? $"🤖 App: `{ApplicationName}`"
                    : $"🤖 App: <code>{ApplicationName.HtmlEncode()}</code>"
            );
        }

        if (logEvent.Exception is null) return new TelegramMessage(text: sb.ToString());

        if (ParseMode == ParseMode.Markdown)
        {
            sb.AppendLine(value: $"\n*{logEvent.Exception.Message}*\n");
            sb.AppendLine(value: $"Message: `{logEvent.Exception.Message}`");
            sb.AppendLine(value: $"Type: `{logEvent.Exception.GetType().Name}`\n");
            sb.AppendLine(value: $"Stack Trace\n```{logEvent.Exception}```");
        }
        else
        {
            sb.AppendLine(value: $"\n<b>{logEvent.Exception.Message.HtmlEncode()}</b>\n");
            sb.AppendLine(value: $"Message: <code>{logEvent.Exception.Message.HtmlEncode()}</code>");
            sb.AppendLine(value: $"Type: <code>{logEvent.Exception.GetType().Name}</code>\n");
            sb.AppendLine(value: $"Stack Trace\n<code>{logEvent.Exception.ToString().HtmlEncode()}</code>");
        }

        return new TelegramMessage(text: sb.ToString());
    }

    private static string GetEmoji(LogEvent log)
    {
        return log.Level switch
        {
            LogEventLevel.Debug => "👉",
            LogEventLevel.Error => "❗",
            LogEventLevel.Fatal => "‼",
            LogEventLevel.Information => "ℹ",
            LogEventLevel.Verbose => "⚡",
            LogEventLevel.Warning => "⚠",
            _ => "",
        };
    }

    protected void SendMessage(string token, string chatId, TelegramMessage message)
    {
        SelfLog.WriteLine($"Trying to send message to chatId '{chatId}': '{message}'.");

        var client = new TelegramBot(botToken: token, timeoutSeconds: 5);
        var sendMessageTask = client.PostAsync(message, chatId, ParseMode);
        sendMessageTask.Wait();

        if (sendMessageTask.Result != null)
            SelfLog.WriteLine($"Message sent to chatId '{chatId}': '{sendMessageTask.Result.StatusCode}'.");
    }
}