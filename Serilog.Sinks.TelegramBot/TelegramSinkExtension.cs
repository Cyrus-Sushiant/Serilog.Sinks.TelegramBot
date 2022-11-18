using Serilog.Configuration;
using Serilog.Events;
using System;

namespace Serilog.Sinks.TelegramBot
{
    public static class TelegramSinkExtension
    {
        public static LoggerConfiguration TelegramBot(
            this LoggerSinkConfiguration loggerConfiguration,
            string token,
            string chatId,
            string applicationName = null,
            TelegramSink.RenderMessageMethod renderMessageImplementation = null,
            LogEventLevel restrictedToMinimumLevel = LevelAlias.Minimum,
            IFormatProvider formatProvider = null,
            ParseMode parseMode = ParseMode.Markdown)
        {
            if (loggerConfiguration == null)
                throw new ArgumentNullException(paramName: nameof(loggerConfiguration));

            return loggerConfiguration.Sink(
                logEventSink: new TelegramSink(
                    chatId: chatId,
                    token: token,
                    applicationName: applicationName,
                    parseMode: parseMode,
                    renderMessageImplementation: renderMessageImplementation,
                    formatProvider: formatProvider),
                restrictedToMinimumLevel: restrictedToMinimumLevel);
        }
    }
}