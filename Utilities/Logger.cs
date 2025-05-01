using NLog;

namespace SchoolAPI_TestProject.Utilities
{
    public static class Logger
    {
        public static readonly ILogger Log = LogManager.GetCurrentClassLogger();
    }
}
