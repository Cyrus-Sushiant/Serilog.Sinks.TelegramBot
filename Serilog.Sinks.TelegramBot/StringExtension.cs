using System.Web;

namespace Serilog.Sinks.TelegramBot
{
    internal static class StringExtension
    {
        internal static string HtmlEncode(this string htmlStr)
        {
            return HttpUtility.HtmlEncode(htmlStr);
        }
    }
}
