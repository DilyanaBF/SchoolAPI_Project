using AventStack.ExtentReports;
using SchoolAPI_TestProject.Rest.Calls;
using SchoolAPI_TestProject.Rest.DataManagement;
using Reqnroll;
using RestSharp;
using Newtonsoft.Json.Linq;
using System.Net;
using SchoolAPI_TestProject.Utilities;
using NLog;



namespace SchoolAPI_TestProject.Tests.BBDTests.StepDefinitions
{
    [Binding]
    public class TeacherStepDefinitions
    {

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
            int statusCode = extractResponseData.ExtractHttpStatusCode(response);
            LogAndReportHelper.AssertHttpOkOrFail(statusCode, _test, logger);
            string tokenValue = extractResponseData.ExtractLoggedInUserToken(response.Content, "access_token");
            string detail = extractResponseData.ExtractResponseDetail(response.Content, "detail");
            

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

            RestResponse response = restCalls.CreateClassWithSubjects(class_name, subject_1, subject_2, subject_3, userToken);

            int classResponseCode = extractResponseData.ExtractHttpStatusCode(response);
            LogAndReportHelper.AssertHttpOkOrFail(classResponseCode, _test, logger);
            string classResponseBody = extractResponseData.ExtractFullResponse(response.Content);
            _scenarioContext.Add("classResponseBody", classResponseBody);

        }

        [Then(@"validate class creation response")]
        public void ValidateClassCreationResponse()
        {
            int statusCode = _scenarioContext.Get<int>("classResponseCode");
            string responseContent = _scenarioContext.ContainsKey("classResponseBody")
                ? _scenarioContext.Get<string>("classResponseBody")
                : "No full response found.";

            _test.Info("Response content: " + responseContent);
            logger.Info("Response content: " + responseContent);

            if (statusCode != 200)
            {
                LogAndReportHelper.Fail($"Expected HTTP 200 but got {statusCode}.", _test, logger);
                return;
            }

            else if (responseContent.Contains("\"message\": \"Class created\"") && responseContent.Contains("\"class_id\":"))
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

            var queryParams = new Dictionary<string, string>
            {
                { "name", "Ivo"},
                { "class_id", "8ae59179-2c90-4c81-92f0-aeeaec526cfb" }
            };

            RestResponse response = restCalls.GeneralRestCall(
                endpoint: "/classes/add_student",
                method: Method.Post,
                queryParams: queryParams,
                bearerToken: userToken
            );
            int statusCode = extractResponseData.ExtractHttpStatusCode(response);
            LogAndReportHelper.AssertHttpOkOrFail(statusCode, _test, logger);

            _scenarioContext.Add("StudentResponse", response.Content);
            _scenarioContext.Add("studentResponseCode", statusCode);
        }

        [Then(@"validate student is added")]
        public void ValidateStudentAdded()
        {
            string responseContent = _scenarioContext.Get<string>("StudentResponse");
            int studentResponseCode = _scenarioContext.Get<int>("studentResponseCode");

            if (studentResponseCode != 200)
            {
                LogAndReportHelper.Fail($"Expected HTTP 200 but got {studentResponseCode}.", _test, logger);
                return;
            }

            else if (responseContent.Contains("\"message\":\"Student added\"") && responseContent.Contains("\"student_id\":"))
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

            _scenarioContext.Add("addGradeResponse", response.Content);
        }

        [Then(@"validate grade is updated")]
        public void GradeAdded()
        {
            string responseContent = _scenarioContext.Get<string>("addGradeResponse");

            if (responseContent.Contains("\"message\":\"Grade updated\""))
            {

                LogAndReportHelper.Success("Grade was updated successfully", _test, logger);
            }
            else
            {
                string detail = extractResponseData.ExtractResponseDetail(responseContent, "detail");
                int statusCode = (int)HttpStatusCode.BadRequest; 

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
            _scenarioContext.Add("classFullResponse", response.Content); 

        }

    }
}
