using AventStack.ExtentReports;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using Reqnroll;

namespace SchoolAPI_TestProject.Utilities
{
    public class UtilitiesMethods
    {
        private static Random random = new Random();

        public static void AssertEqual<T>(T expected, T actual, string message, ScenarioContext scenarioContext)
        {
            ExtentTest _test = scenarioContext.Get<ExtentTest>("ExtentTest");

            if (!EqualityComparer<T>.Default.Equals(expected, actual))
            {
                Logger.Log.Error(message + Environment.NewLine + $"Expected: {expected}, but was: {actual}");
                _test.Log(Status.Fail, message + Environment.NewLine + $"Expected: {expected}, but was: {actual}");
                Assert.Fail(message + Environment.NewLine + $"Expected: {expected}, but was: {actual}");
            }
        }

        /// <summary>
        /// Log message into the report log and test run log file
        /// </summary>
        /// <param name="message">Message to be logged</param>
        /// <param name="scenarioContext">Scenario context</param>
        /// <param name="status">Type of log message, e.g. Info, Debug</param>
        public static void LogMessage(
            string message,
            ScenarioContext scenarioContext,
            LogStatuses status = LogStatuses.Info)
        {
            ExtentTest _test = scenarioContext.Get<ExtentTest>("ExtentTest");
            if (status == LogStatuses.Info)
            {
                _test.Log(Status.Info, message);
                Logger.Log.Info(message);
            }
            else if (status == LogStatuses.Warning)
            {
                _test.Log(Status.Warning, message);
                Logger.Log.Warn(message);
            }
            else if (status == LogStatuses.Debug)
            {
                _test.Log(Status.Info, message);
                Logger.Log.Debug(message);
            }
        }

        public static string GenerateOrderNumber()
        {
            string prefix = "ORD";
            string datePart = DateTime.Now.ToString("yyyyMMdd");
            string randomPart = GenerateRandomString(6);
            return $"{prefix}-{datePart}-{randomPart}";
        }

        private static string GenerateRandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            char[] result = new char[length];

            for (int i = 0; i < length; i++)
            {
                result[i] = chars[random.Next(chars.Length)];
            }

            return new string(result);
        }
    }

    public enum LogStatuses
    {
        Info,
        Warning,
        Debug
    }

    
}
