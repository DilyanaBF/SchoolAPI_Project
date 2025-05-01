using AventStack.ExtentReports.Reporter;
using AventStack.ExtentReports;

namespace SchoolAPI_TestProject.Utilities
{
    public static class ExtentManager
    {
        private static ExtentReports _extent;
        private static ThreadLocal<ExtentTest> _test = new ThreadLocal<ExtentTest>();

        public static ExtentReports InitReport()
        {
            if (_extent == null)
            {
                var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                var htmlReporter = new ExtentSparkReporter($"TestReport_{timestamp}.html");
                //htmlReporter.LoadConfig("extent-config.xml");
                htmlReporter.Config.DocumentTitle = "Test Execution Report";
                htmlReporter.Config.ReportName = "Functional Tests";
                htmlReporter.Config.TimelineEnabled = true;

                _extent = new ExtentReports();
                _extent.AttachReporter(htmlReporter);
            }
            return _extent;
        }

        public static ExtentTest CreateTest(string testName)
        {
            ExtentTest test = _extent.CreateTest(testName);
            _test.Value = test;
            return test;
        }

        public static ExtentTest GetTest()
        {
            return _test.Value;
        }

        public static void FlushReport()
        {
            _extent.Flush();
        }
    }
}
