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
using OpenQA.Selenium;


namespace SchoolAPI_TestProject.Tests.BBDTests.StepDefinitions
{
    [Binding]
    public class LoginStepDefinitions
    {
        private RestCalls restCalls = new RestCalls();
        private ResponseDataExtractors extractResponseData = new ResponseDataExtractors();

        private readonly ScenarioContext _scenarioContext;
        private ExtentTest _test;
        private static readonly ILogger logger = LogManager.GetCurrentClassLogger();

        public LoginStepDefinitions(ScenarioContext scenarioContext)
        {
            _scenarioContext = scenarioContext;
            _test = scenarioContext.Get<ExtentTest>("ExtentTest");
        }

        [Given(@"user with role (.*) logs in with valid username (.*) and password (.*)")]
        public void LoginWithValidcredentials(string userRole, string username, string password)
        {
            string role = userRole;
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
            string fullResponse = extractResponseData.ExtractFullResponse(response.Content);


            _scenarioContext.Add("FullResponse", fullResponse);
            _scenarioContext.Add("UserToken", tokenValue);
            _scenarioContext.Add("StatusCode", statusCode);


        }

        [Then(@"user is logged in successfully")]
        public void ValidateUserIsLoggedIn()
        {
            string token = _scenarioContext.Get<string>("UserToken");
            int loginStatusCode = _scenarioContext.Get<int>("StatusCode");
            string fullResponse = _scenarioContext.ContainsKey("FullResponse")
                ? _scenarioContext.Get<string>("FullResponse")
                : "No full response found.";

            LogAndReportHelper.Info($"🌐 HTTP Status Code: {loginStatusCode}", _test, logger);
            LogAndReportHelper.Info($"📄 Full response: {fullResponse}", _test, logger);


            if (string.IsNullOrEmpty(token))
            {
                LogAndReportHelper.Fail(
                    $"Token is missing. Expected HTTP status code 200 (OK), but got {loginStatusCode}. Full response: {fullResponse}",
                        _test,
                        logger
                    );

                return;
            }

            LogAndReportHelper.Success($"Token successfully extracted: {token}", _test, logger);
        }

        [Given(@"user logs in with invalid username (.*) and password (.*)")]
        public void LoginWithInvalidcredentials(string username, string password)
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
            //string fullResponse = _scenarioContext.ContainsKey("FullResponse")
            //    ? _scenarioContext.Get<string>("FullResponse")
            //    : "No full response found.";
            string fullResponse = extractResponseData.ExtractFullResponse(response.Content);


            _scenarioContext.Add("FullResponse", fullResponse);
            //_scenarioContext.Add("FullResponse", fullResponse);
            _scenarioContext.Add("UserToken", tokenValue);
            _scenarioContext.Add("StatusCode", statusCode);


        }

        [Then(@"the user is not logged in")]
        public void ValidateUserIsNotLoggedIn()
        {
            string token = _scenarioContext.Get<string>("UserToken");
            int loginStatusCode = _scenarioContext.Get<int>("StatusCode");
            string fullResponse = _scenarioContext.ContainsKey("FullResponse")
                ? _scenarioContext.Get<string>("FullResponse")
                : "No full response found.";

            LogAndReportHelper.Info($"🌐 HTTP Status Code: {loginStatusCode}", _test, logger);
            LogAndReportHelper.Info($"📄 Full response: {fullResponse}", _test, logger);


            if (!string.IsNullOrEmpty(token))
            {
                LogAndReportHelper.Fail(
                    $"Token is missing. Expected HTTP status code 401, but got {loginStatusCode}. Token was extracted: {token}",
                        _test,
                        logger
                    );

                return;
            }
            if (loginStatusCode == 401)
            {

                LogAndReportHelper.Success($"User was not logged in successfully." +
                    $"Response status code was {loginStatusCode}. Full Response: {fullResponse}",
                    _test,
                    logger);
            }


        }
    }
}
