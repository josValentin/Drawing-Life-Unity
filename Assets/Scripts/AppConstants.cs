public class AppConstants
{
    public const string WEB_SERVICE = "https://drawing-life-backend.onrender.com";
    public const string API_PROCESS_IMAGE = "/colorize";
    public const string API_PING = "/ping";

    public static string ProcessImageUrl => WEB_SERVICE + API_PROCESS_IMAGE;
    public static string PingUrl => WEB_SERVICE + API_PING;
}