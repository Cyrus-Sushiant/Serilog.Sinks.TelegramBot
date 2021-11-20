using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;

namespace Serilog.Sinks.TelegramBot
{
    public class TelegramBot
    {
        private readonly Uri _apiUrl;
        private readonly HttpClient _httpClient = new HttpClient();

        public TelegramBot(string botToken, int timeoutSeconds = 10)
        {
            if (string.IsNullOrEmpty(value: botToken))
                throw new ArgumentException(message: "Bot token can't be empty", paramName: nameof(botToken));

            _apiUrl = new Uri(uriString: $"https://api.telegram.org/bot{botToken}/sendMessage");
            _httpClient.Timeout = TimeSpan.FromSeconds(value: timeoutSeconds);
        }

        public async Task<HttpResponseMessage> PostAsync(TelegramMessage message, string chatId, ParseMode parseMode)
        {
            var payload = new
            {
                chat_id = chatId,
                text = message.Text,
                parse_mode = parseMode.ToString()
            };
            var json = JsonSerializer.Serialize(value: payload);
            var response = await _httpClient.PostAsync(requestUri: _apiUrl,
                content: new StringContent(content: json, encoding: Encoding.UTF8, mediaType: "application/json"));

            return response;
        }
    }
}