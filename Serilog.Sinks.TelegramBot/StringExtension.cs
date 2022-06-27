using System.Web;

namespace Serilog.Sinks.TelegramBot
{
    public static class StringExtension
    {
        public static string HtmlEncode(this string htmlStr)
        {
            return HttpUtility.HtmlEncode(htmlStr);
        }
    }
}
