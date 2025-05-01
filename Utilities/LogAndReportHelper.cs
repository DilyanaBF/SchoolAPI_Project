using AventStack.ExtentReports;
using NLog;
using NUnit.Framework;
using NUnit.Framework.Legacy;


namespace SchoolAPI_TestProject.Utilities
{
    public static class LogAndReportHelper
    {
        
        public static void AssertEqual(int expected, int actual, string message, ExtentTest test, ILogger logger)
        {
            if (expected != actual)
            {
                test?.Log(Status.Fail, message);
                logger?.Error(message);
            }
            ClassicAssert.AreEqual(expected, actual, message);
        }

        public static void AssertEqualString(string expected, string actual, string message, ExtentTest test, ILogger logger)
        {
            if (expected != actual)
            { 
                test?.Log(Status.Fail, message);
                logger?.Error(message);
            }
            ClassicAssert.AreEqual(expected, actual, message);
        }

        public static void AssertTrue(bool condition, string message, ExtentTest test, ILogger logger)
        {
            if (!condition)
            {
                test?.Log(Status.Fail, message);
                logger?.Error(message);
            }
            Assert.That(condition, message);
        }

        public static void AssertFalse(bool condition, string message, ExtentTest test, ILogger logger)
        {
            if (condition)
            {
                test?.Log(Status.Fail, message);
                logger?.Error(message);
            }
            Assert.That(condition, message);
        }

        public static void Info(string message, ExtentTest test, ILogger logger)
        {
            //test?.Log(Status.Info, message);
            logger?.Info(message);
        }

        public static void Success(string message, ExtentTest test, ILogger logger)
        {
            test?.Log(Status.Pass, message);
            logger?.Info("✅ " + message);
        }

        public static void Fail(string message, ExtentTest test, ILogger logger)
        {
            test?.Log(Status.Fail, message);
            logger?.Error("❌ " + message);
            Assert.Fail(message);
        }
        public static void LogException(string message, Exception ex, ExtentTest test, ILogger logger)
        {
            string fullMessage = $"{message} - Exception: {ex.Message}";
            test?.Log(Status.Error, fullMessage);
            logger?.Error(ex, message); // Logs full stack trace
        }

        public static void AssertNotNull(object actual, string message, ExtentTest test, ILogger logger)
        {
            if (actual == null)
            {
                test?.Log(Status.Fail, message);
                logger?.Error(message);
            }
            ClassicAssert.IsNotNull(actual, message);
        }

        public static void AssertHttpOkOrFail(int statusCode, ExtentTest test, ILogger logger)
        {
            if (statusCode != 200)
            {
                string msg = $"❌ Expected HTTP 200 but got {statusCode}.";
                test?.Fail(msg);
                logger?.Error(msg);
                Assert.Fail(msg);
            }
        }
    }
}