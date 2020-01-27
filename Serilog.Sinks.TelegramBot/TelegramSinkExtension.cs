using System;
using Serilog.Configuration;
using Serilog.Events;

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
            IFormatProvider formatProvider = null
        )
        {
            if (loggerConfiguration == null)
                throw new ArgumentNullException(paramName: nameof(loggerConfiguration));

            return loggerConfiguration.Sink(
                logEventSink: new TelegramSink(
                    chatId: chatId,
                    token: token,
                    applicationName: applicationName,
                    renderMessageImplementation: renderMessageImplementation,
                    formatProvider: formatProvider
                ),
                restrictedToMinimumLevel: restrictedToMinimumLevel);
        }
    }
}