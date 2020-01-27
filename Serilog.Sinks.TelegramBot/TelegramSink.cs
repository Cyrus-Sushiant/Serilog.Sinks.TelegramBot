using System;
using System.Text;
using System.Threading.Tasks;
using Serilog.Core;
using Serilog.Debugging;
using Serilog.Events;

namespace Serilog.Sinks.TelegramBot
{
    public class TelegramSink : ILogEventSink
    {
        /// <summary>
        /// Delegate to allow overriding of the RenderMessage method.
        /// </summary>
        public delegate TelegramMessage RenderMessageMethod(LogEvent input);

        private static string _applicationName;
        private readonly string _chatId;
        private readonly string _token;
        protected readonly IFormatProvider FormatProvider;

        /// <summary>
        /// RenderMessage method that will transform LogEvent into a Telegram message.
        /// </summary>
        protected RenderMessageMethod RenderMessageImplementation = RenderMessage;

        public TelegramSink(string chatId, string token, string applicationName, RenderMessageMethod renderMessageImplementation,
            IFormatProvider formatProvider)
        {
            if (string.IsNullOrWhiteSpace(value: chatId))
                throw new ArgumentNullException(paramName: nameof(chatId));

            if (string.IsNullOrWhiteSpace(value: token))
                throw new ArgumentNullException(paramName: nameof(token));

            FormatProvider = formatProvider;
            if (renderMessageImplementation != null)
                RenderMessageImplementation = renderMessageImplementation;
            _applicationName = applicationName;
            _chatId = chatId;
            _token = token;
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
            sb.AppendLine(value: $"{GetEmoji(log: logEvent)} {logEvent.RenderMessage()}");

            if (!string.IsNullOrEmpty(_applicationName))
            {
                sb.AppendLine(value: $"🤖 App: `{_applicationName}`");
            }

            if (logEvent.Exception != null)
            {
                sb.AppendLine(value: $"\n*{logEvent.Exception.Message}*\n");
                sb.AppendLine(value: $"Message: `{logEvent.Exception.Message}`");
                sb.AppendLine(value: $"Type: `{logEvent.Exception.GetType().Name}`\n");
                sb.AppendLine(value: $"Stack Trace\n```{logEvent.Exception}```");
            }
            return new TelegramMessage(text: sb.ToString());
        }

        private static string GetEmoji(LogEvent log)
        {
            switch (log.Level)
            {
                case LogEventLevel.Debug:
                    return "👉";
                case LogEventLevel.Error:
                    return "❗";
                case LogEventLevel.Fatal:
                    return "‼";
                case LogEventLevel.Information:
                    return "ℹ";
                case LogEventLevel.Verbose:
                    return "⚡";
                case LogEventLevel.Warning:
                    return "⚠";
                default:
                    return "";
            }
        }

        protected void SendMessage(string token, string chatId, TelegramMessage message)
        {
            SelfLog.WriteLine($"Trying to send message to chatId '{chatId}': '{message}'.");

            var client = new TelegramBot(botToken: token, timeoutSeconds: 5);

            var sendMessageTask = client.PostAsync(message: message, chatId: chatId);
            Task.WaitAll(sendMessageTask);

            var sendMessageResult = sendMessageTask.Result;
            if (sendMessageResult != null)
                SelfLog.WriteLine($"Message sent to chatId '{chatId}': '{sendMessageResult.StatusCode}'.");
        }
    }
}