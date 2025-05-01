using AventStack.ExtentReports;
using SchoolAPI_TestProject.Rest.Calls;
using SchoolAPI_TestProject.Rest.DataManagement;
using Newtonsoft.Json;
using Reqnroll;
using RestSharp;
using Newtonsoft.Json.Linq;
using System.Net;
using SchoolAPI_TestProject.Utilities;
using NLog;
using OpenQA.Selenium.BiDi.Modules.Network;


namespace SchoolAPI_TestProject.Tests.BBDTests
{
    [Binding]
    public class TeacherStepDefinitions
    {
       // private readonly ScenarioContext _context;
       // private readonly HttpClient _client;
       // private string? _authToken;
       // private string? _classId;

        private RestCalls restCalls = new RestCalls();
        private ResponseDataExtractors extractResponseData = new ResponseDataExtractors();

        private readonly ScenarioContext _scenarioContext;
        private ExtentTest _test;
        private static readonly ILogger logger = LogManager.GetCurrentClassLogger();

        public TeacherStepDefinitions(ScenarioContext scenarioContext)
        {
            _scenarioContext = scenarioContext;
            _test = scenarioContext.Get<ExtentTest>("ExtentTest");
        }

        [Given(@"login with ""(.*)"" username and ""(.*)"" password")]
        public void Login(string username, string password)
        {
            RestResponse response = restCalls.LoginSchoolAPI(username, password);
            string tokenValue = extractResponseData.ExtractLoggedInUserToken(response.Content, "access_token");
            string detail = extractResponseData.ExtractResponseDetail(response.Content, "detail");
            int statusCode = extractResponseData.ExtractHttpStatusCode(response);

            _scenarioContext.Add("FullResponse", response.Content);
            _scenarioContext.Add("UserToken", tokenValue);
            _scenarioContext.Add("StatusCode", statusCode);
        }

        [When(@"the teacher creates a new ""(.*)"" class_name with ""(.*)"" subject_1, ""(.*)"" subject_2, ""(.*)"" subject_3")]
        public void CreateNewClass(string class_name, string subject_1, string subject_2, string subject_3)
        {
            string userToken = _scenarioContext.Get<string>("UserToken");

            if (string.IsNullOrEmpty(userToken))
            {
                LogAndReportHelper.Fail("User was not logged in successfully. Stopping test execution.", _test, logger);
                return;
            }

            // Call the API and store the full response
            RestResponse response = restCalls.CreateClassWithSubjects(class_name, subject_1, subject_2, subject_3, userToken);

            _scenarioContext["classResponseCode"] = (int)response.StatusCode;
            _scenarioContext["classResponseBody"] = response.Content;

        }

        [Then(@"validate class creation response1")]
        public void ValidateClassCreationResponse1()
        {
            int statusCode = _scenarioContext.Get<int>("classResponseCode");
            string responseContent = _scenarioContext.Get<string>("classResponseBody");

            // Always log raw response
            _test.Info("Response content: " + responseContent);
            logger.Info("Response content: " + responseContent);

            // Validate status code
            if (statusCode != 200)
            {
                LogAndReportHelper.Fail($"Expected HTTP 200 but got {statusCode}.", _test, logger);
            }

           if (responseContent.Contains("\"message\":\"Class created\"") && responseContent.Contains("\"class_id\":"))
            {

                LogAndReportHelper.Success("Class was created successfully", _test, logger);
            }
            else
            {
                
                string detail = extractResponseData.ExtractResponseDetail(responseContent, "detail");
                
                if (string.IsNullOrEmpty(detail))
                {
                    detail = "No detail message returned.";
                }

                LogAndReportHelper.Fail($"Failed to add class. Status Code: {statusCode}. Detail: {detail}", _test, logger);
            }
        }

        [When(@"the teacher adds a new student")]
        public void AddNewStudent()
        {
            string userToken = _scenarioContext.Get<string>("UserToken");

            // Student details
            var queryParams = new Dictionary<string, string>
            {
                { "name", "Ivo"},
                { "class_id", "ecadac35-dd50-4120-b876-411ec0d51cd9" }
            };

            RestResponse response = restCalls.GeneralRestCall(
                endpoint: "/classes/add_student",
                method: Method.Post,
                queryParams: queryParams,
                bearerToken: userToken
            );

            // Store the response for later validation
            _scenarioContext.Add("StudentResponse", response.Content);
        }

        [Then(@"validate student is added")]
        public void ValidateStudentAdded()
        {
            string responseContent = _scenarioContext.Get<string>("StudentResponse");

            if (responseContent.Contains("\"message\":\"Student added\"") && responseContent.Contains("\"student_id\":"))
            {

                LogAndReportHelper.Success("Student was added successfully", _test, logger);
            }
            else
            {

                string detail = extractResponseData.ExtractResponseDetail(responseContent, "detail");

                if (string.IsNullOrEmpty(detail))
                {
                    detail = "No detail message returned.";
                }

                LogAndReportHelper.Fail($"Failed to add student. Detail: {detail}", _test, logger);
            }
        }

        [When(@"the teacher updates grade for a student per subject")]
        public void AddGrade()
        {
            string userToken = _scenarioContext.Get<string>("UserToken");

            // Student details
            var queryParams = new Dictionary<string, string>
            {
                { "student_id", "756dd19f-8d83-4f64-b53a-0d6b14a4dd68"},
                { "subject", "History" },
                { "grade", "3" }
            };

            RestResponse response = restCalls.GeneralRestCall(
                endpoint: "/grades/add",
                method: Method.Put,
                queryParams: queryParams,
                bearerToken: userToken
            );

            // Store the response for later validation
            _scenarioContext.Add("addGradeResponse", response.Content);
        }

        [Then(@"validate grade is updated")]
        public void GradeAdded()
        {
            string responseContent = _scenarioContext.Get<string>("addGradeResponse");

            // Check for success response
            if (responseContent.Contains("\"message\":\"Grade updated\""))
            {

                LogAndReportHelper.Success("Grade was added successfully", _test, logger);
            }
            else
            {
                // In case of failure, extract the detail and status code
                string detail = extractResponseData.ExtractResponseDetail(responseContent, "detail");
                int statusCode = (int)HttpStatusCode.BadRequest; // You might want to extract actual status from the response

                if (string.IsNullOrEmpty(detail))
                {
                    detail = "No detail message returned.";
                }

                LogAndReportHelper.Fail($"Failed to add grade. Status Code: {statusCode}. Detail: {detail}", _test, logger);
            }
        }

        [When(@"the 1teacher creates a new ""(.*)"" class_name with ""(.*)"" subject_1, ""(.*)"" subject_2, ""(.*)"" subject_3")]
        public void CreateNewClass12(string class_name, string subject_1, string subject_2, string subject_3)
        {
            string userToken = _scenarioContext.Get<string>("UserToken");


            RestResponse response = restCalls.CreateClassWithSubjects(class_name, subject_1, subject_2, subject_3, userToken);

            string responseMessage = extractResponseData.ExtractResponseMessage(response.Content, "message");
            string responseDetail = extractResponseData.ExtractResponseDetail(response.Content, "detail");
            string classId = extractResponseData.ExtractResponseMessage(response.Content, "class_id");

            _scenarioContext.Add("message", responseMessage);
            _scenarioContext.Add("detail", responseDetail);
            _scenarioContext.Add("classId", classId);
            _scenarioContext.Add("classFullResponse", response.Content); // Add full response for debugging

        }


        [Then(@"validate class creation response2")]
        public void ValidateClassCreationResponse2()
        {
            try
            {
                //int statusCode = _scenarioContext.Get<int>("statusCode");
                string classFullResponse = _scenarioContext.Get<string>("classFullResponse");
                //string latestFullResponse = _scenarioContext.Get<string>("latestFullResponse");

                JObject json = JObject.Parse(classFullResponse);
                string classId = json["class_id"]?.ToString();
                string message = json["message"]?.ToString();

                //LogAndReportHelper.Info($"🌐 HTTP Status Code: {statusCode}", _test, logger);
                //LogAndReportHelper.Info($"📦 Full Response: {fullResponse}", _test, logger);

                // Assertions
                //LogAndReportHelper.AssertEqual(200, statusCode, "Expected HTTP status code 200 (OK).", _test, logger);
                LogAndReportHelper.AssertFalse(string.IsNullOrEmpty(classId), "Expected a non-empty 'class_id' in response.", _test, logger);
                LogAndReportHelper.AssertEqualString("Class created", message, "Expected message 'Class created' in response.", _test, logger);

                LogAndReportHelper.Success($"✅ Class successfully created with ID: {classId}", _test, logger);
            }
            catch (Exception ex)
            {
                LogAndReportHelper.LogException("An error occurred while validating the class creation response.", ex, _test, logger);
                throw;
            }
        }

    }
}
