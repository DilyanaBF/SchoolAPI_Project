using AventStack.ExtentReports;
using NLog;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolAPI_TestProject.Utilities
{
    public class Utils
    {
        public static void AssertMethodWithReport(int expected, int actual, string errorMessage, ExtentTest testReportObject)
        {
            if (actual != expected)
            {
                testReportObject.Log(Status.Fail, errorMessage);
            }
            Assert.That(actual, Is.EqualTo(expected), errorMessage);
        }

        public static void AssertWithReportAndLog(int expected, int actual, string errorMessage, ExtentTest testReportObject, ILogger Logger)
        {
            if (actual != expected)
            {
                testReportObject.Log(Status.Fail, errorMessage);
                Logger.Error(errorMessage);
            }
            Assert.That(actual, Is.EqualTo(expected), errorMessage);
        }

        public static void logSuccessWithReportAndLog(string successMessage, ExtentTest testReportObject, ILogger Logger)
        {
            testReportObject.Log(Status.Info, successMessage);
            Logger.Info(successMessage);
        }
    }
}
