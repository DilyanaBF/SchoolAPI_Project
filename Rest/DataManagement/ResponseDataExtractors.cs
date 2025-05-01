using AventStack.ExtentReports;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using RestSharp;

namespace SchoolAPI_TestProject.Rest.DataManagement
{
    public class ResponseDataExtractors
    {

        public string ExtractLoggedInUserToken(string jsonResponse, string jsonIdentfier = "token")
        {
            JObject jsonObject = JObject.Parse(jsonResponse);
            return jsonObject[jsonIdentfier]?.ToString();
        }

        public string ExtractResponseDetail(string jsonResponse, string jsonIdentfier = "detail")
        {
            JObject jsonObject = JObject.Parse(jsonResponse);
            return jsonObject[jsonIdentfier]?.ToString();
        }

        public string ExtractResponseMessage(string jsonResponse, string jsonIdentfier = "message")
        {
            JObject jsonObject = JObject.Parse(jsonResponse);
            return jsonObject[jsonIdentfier]?.ToString();
        }

        public int ExtractHttpStatusCode(RestResponse response)
        {
            if (response == null)
                throw new ArgumentNullException(nameof(response), "Response cannot be null.");

            return (int)response.StatusCode;
        }
        public string ExtractFullResponse(string jsonResponse)
        {
            JObject jsonObject = JObject.Parse(jsonResponse);
            return jsonObject.ToString();
        }

    }
}
