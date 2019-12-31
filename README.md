# Serilog.Sinks.TelegramBot

[![NuGet Version](https://img.shields.io/nuget/v/Serilog.Sinks.TelegramBot.svg?style=flat)](https://www.nuget.org/packages/Serilog.Sinks.TelegramBot/)
[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)](https://raw.githubusercontent.com/Cyrus-Sushiant/Serilog.Sinks.TelegramBot/master/LICENSE)

A Serilog sink that writes events as messages to [Telegram](https://telegram.org/).

All you need is bot token and chat id. To manage bots go to [Bot Father](https://telegram.me/botfather). After you got bot token add bot to contacts and start it (`/start`). To get your id navigate to https://api.telegram.org/botTOKEN/getUpdates

## Available for
* .NetCore 3.1

# Install
```
Install-Package Serilog.Sinks.TelegramBot
```

## Basic usage:
```csharp
var log = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .WriteTo.TelegramBot("000000:AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA", "000000")
                .CreateLogger();
log.Information("This is an information message!");
```

### Example

![Simple Message](/assets/example.png)
