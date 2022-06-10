using System.Web;

namespace Serilog.Sinks.TelegramBot
{
    public static class StringHelper
    {
        public static string HtmlEncode(this string htmlStr)
        {
            return HttpUtility.HtmlEncode(htmlStr);
        }
    }
}
