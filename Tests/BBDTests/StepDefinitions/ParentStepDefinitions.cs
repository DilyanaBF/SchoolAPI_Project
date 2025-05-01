using AventStack.ExtentReports;
using SchoolAPI_TestProject.Rest.Calls;
using SchoolAPI_TestProject.Rest.DataManagement;
using Newtonsoft.Json;
using Reqnroll;
using RestSharp;
using Newtonsoft.Json.Linq;
using NUnit.Framework.Legacy;
using System.Net;
using SchoolAPI_TestProject.Utilities;
using NLog;


namespace SchoolAPI_TestProject.Tests.BBDTests.StepDefinitions
{
    [Binding]
    public class ParentStepDefinitions
    {
        private RestCalls restCalls = new RestCalls();
        private ResponseDataExtractors extractResponseData = new ResponseDataExtractors();
        private readonly ScenarioContext _scenarioContext;
        private ExtentTest _test;

        private static readonly ILogger logger = LogManager.GetCurrentClassLogger();

        public ParentStepDefinitions(ScenarioContext scenarioContext)
        {
            _scenarioContext = scenarioContext;
            _test = scenarioContext.Get<ExtentTest>("ExtentTest");
        }

        [Given(@"parent login with ""(.*)"" username and ""(.*)"" password")]
        public void Login(string username, string password)
        {

            var formParams = new Dictionary<string, string>
            {
                { "username", username },
                { "password", password }
            };

            RestResponse response = restCalls.GeneralRestCall(
                endpoint: "/auth/login",
                method: Method.Post,
                formParams: formParams,
                contentType: "application/x-www-form-urlencoded"
            );

            int statusCode = extractResponseData.ExtractHttpStatusCode(response);
            string tokenValue = extractResponseData.ExtractLoggedInUserToken(response.Content, "access_token");
            string detail = extractResponseData.ExtractResponseDetail(response.Content, "detail");


            _scenarioContext.Add("UserToken", tokenValue);
            _scenarioContext.Add("StatusCode", statusCode);

            if (string.IsNullOrEmpty(tokenValue))
            {
                LogAndReportHelper.Fail(
                    $"User did not log successfully. Expected HTTP status code 200 (OK), but got {statusCode}.",
                        _test,
                        logger
                    );

                return;
            }

            LogAndReportHelper.Info("User logged in successfully", _test, logger);
        }

        [When(@"a parent checks own child grades")]
        public void GetGrades()
        {
            string userToken = _scenarioContext.Get<string>("UserToken");

            RestResponse response = restCalls.GeneralRestCall(
                endpoint: "grades/student/43bac5dc-ecba-4826-8b8d-204cecd07b18",
                method: Method.Get,
                bearerToken: userToken
            );

            int gradesStatusCode = extractResponseData.ExtractHttpStatusCode(response);
            _scenarioContext.Add("gradesResponse", response.Content);
            _scenarioContext.Add("gradesResponsCode", gradesStatusCode);

        }
        [Then(@"validate student grades response is successful")]
        public void ValidateStudentGradesResponse2()
        {
            // Get response status and body from scenario context
            int statusCode = _scenarioContext.Get<int>("gradesResponsCode");
            string responseContent = _scenarioContext.Get<string>("gradesResponse");

            // Always log the raw response
            _test.Info("Response content: " + responseContent);
            logger.Info("Response content: " + responseContent);

            // Fail if status code is not 200
            if (statusCode != 200)
            {
                LogAndReportHelper.Fail($"Expected HTTP 200 but got {statusCode}.", _test, logger);
            }

            try
            {
                // Parse the response body
                var json = JObject.Parse(responseContent);

                // Check for the "grades" array
                var gradesArray = json["grades"] as JArray;
                if (gradesArray == null)
                {
                    LogAndReportHelper.Fail("Missing or invalid 'grades' array in the response.", _test, logger);
                }

                _test.Info($"Grades array found with {gradesArray.Count} item(s).");
                LogAndReportHelper.Success("Student grades response is valid.", _test, logger);
            }
            catch (Exception ex)
            {
                // Unexpected error in JSON parsing or logic — not a validation failure
                LogAndReportHelper.Fail($"Unexpected error during response validation: {ex.Message}", _test, logger);
            }
        }

        [When(@"a parent checks not own child grades")]
        public void GetGradesForotherChild()
        {


            string userToken = _scenarioContext.Get<string>("UserToken");

            RestResponse response = restCalls.GeneralRestCall(
                endpoint: "grades/student/756dd19f-8d83-4f64-b53a-0d6b14a4dd68",
                method: Method.Get,
                bearerToken: userToken
            );

            int noGradesStatusCode = extractResponseData.ExtractHttpStatusCode(response);
            _scenarioContext.Add("noGradesResponse", response.Content);
            _scenarioContext.Add("noGradesResponsCode", noGradesStatusCode);

        }

        [Then(@"the parent is forbidden from accessing another student's grades")]
        public void ValidateParentCannotAccessOtherStudentGrades()
        {
            // Get response status and body from scenario context
            int statusCode = _scenarioContext.Get<int>("noGradesResponsCode");
            string responseContent = _scenarioContext.Get<string>("noGradesResponse");

            // Always log the raw response
            _test.Info("Response status: " + statusCode);
            logger.Info("Response status: " + statusCode);
            _test.Info("Response content: " + responseContent);
            logger.Info("Response content: " + responseContent);

            // Validate status code
            if (statusCode != 403)
            {
                LogAndReportHelper.Fail($"Expected HTTP 403 but got {statusCode}.", _test, logger);
            }

            try
            {
                // Parse and validate error detail
                var json = JObject.Parse(responseContent);
                var detailMessage = json["detail"]?.ToString();

                if (string.IsNullOrEmpty(detailMessage))
                {
                    LogAndReportHelper.Fail("Missing 'detail' message in the response body.", _test, logger);
                }

                var expectedMessage = "You can't view this student's grades";
                if (detailMessage != expectedMessage)
                {
                    LogAndReportHelper.Fail($"Unexpected error message. Expected: '{expectedMessage}', Actual: '{detailMessage}'", _test, logger);
                }

                LogAndReportHelper.Success("Properly forbidden from accessing another student's grades.", _test, logger);
            }
            catch (Exception ex)
            {
                LogAndReportHelper.Fail($"Unexpected error during forbidden access validation: {ex.Message}", _test, logger);
            }
        }

        [Then(@"validate student grades are not returned")]
        public void ValidateStudentGradesNotReturned()
        {
            string response = _scenarioContext.Get<string>("noGradesResponse");
            int responseStatusCode = _scenarioContext.Get<int>("noGradesResponseCode");


            if (string.IsNullOrEmpty(response))
            {
                LogAndReportHelper.Fail("No response content was returned.", _test, logger);
            }

            try
            {
                // Check status code first
                //int statusCode = (int)noGradesResponseCode.StatusCode;
                LogAndReportHelper.AssertEqual(403, responseStatusCode, $"Expected 403 Forbidden, but got {responseStatusCode}.", _test, logger);

                var json = JObject.Parse(response);

                // Ensure the detail message is exactly what we expect
                string? detail = json["detail"]?.ToString();

                LogAndReportHelper.AssertNotNull(detail, "Expected 'detail' in the response but it was missing.", _test, logger);
                LogAndReportHelper.AssertEqualString("You can't view this student's grades", detail, $"Unexpected detail message: {detail}", _test, logger);

                // Fail explicitly if grades exist (just in case!)
                if (json["grades"] != null)
                {
                    NUnit.Framework.Assert.Fail("Grades array was unexpectedly returned in the response.");
                }

                Console.WriteLine("✅ Correctly blocked access to student grades with 403 and proper message.");
            }
            catch (JsonException ex)
            {
                NUnit.Framework.Assert.Fail($"Invalid JSON returned: {ex.Message}\nContent: {response}");
            }
            catch (Exception ex)
            {
                NUnit.Framework.Assert.Fail($"Unexpected error: {ex.Message}\nContent: {response}");
            }
        }



    }
}
