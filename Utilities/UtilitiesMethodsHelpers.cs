using AventStack.ExtentReports;
using SchoolAPI_TestProject.Utilities;
using Reqnroll;

internal static class UtilitiesMethodsHelpers
{

    public static void LogInfoMessage(
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
}