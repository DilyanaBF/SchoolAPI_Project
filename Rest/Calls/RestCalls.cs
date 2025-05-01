using SchoolAPI_TestProject.Utilities;
using Newtonsoft.Json.Linq;
using RestSharp;
using System.Text;

namespace SchoolAPI_TestProject.Rest.Calls
{
    public class RestCalls
    {

        string baseUrl = "https://schoolprojectapi.onrender.com/";

        public RestResponse GeneralRestCall(
            string endpoint,
            Method method,
            Dictionary<string, string>? queryParams = null,
            Dictionary<string, string>? formParams = null,
            object? jsonBody = null,
            Dictionary<string, string>? headers = null,
            string? bearerToken = null,
            string contentType = "application/json" 
        )
        {
            var options = new RestClientOptions(baseUrl)
            {
                Timeout = TimeSpan.FromSeconds(120),
            };

            var client = new RestClient(options);
            var request = new RestRequest(endpoint, method);

            if (queryParams != null)
            {
                foreach (var param in queryParams)
                {
                    request.AddQueryParameter(param.Key, param.Value);
                }
            }

            if (!string.IsNullOrEmpty(contentType))
            {
                request.AddHeader("Content-Type", contentType);
            }

            if (headers != null)
            {
                foreach (var header in headers)
                {
                    request.AddHeader(header.Key, header.Value);
                }
            }

            if (!string.IsNullOrEmpty(bearerToken))
            {
                request.AddHeader("Authorization", $"Bearer {bearerToken}");
            }

            if (method == Method.Post || method == Method.Put)
            {
                if (formParams != null && contentType == "application/x-www-form-urlencoded")
                {
                    foreach (var param in formParams)
                    {
                        request.AddParameter(param.Key, param.Value);
                    }
                }
                else if (formParams != null && contentType == "multipart/form-data")
                {
                    request.AlwaysMultipartFormData = true;
                    foreach (var param in formParams)
                    {
                        request.AddParameter(param.Key, param.Value);
                    }
                }
                else if (jsonBody != null && contentType == "application/json")
                {
                    request.AddJsonBody(jsonBody);
                }
            }

            var response = client.Execute(request);
            Console.WriteLine($"[RESPONSE] Status: {(int)response.StatusCode} | Content: {response.Content}");
            return response;
            
        }

        public RestResponse LoginSchoolAPI(string username, string password)
        {
            var options = new RestClientOptions("https://schoolprojectapi.onrender.com/")
            {
                Timeout = TimeSpan.FromSeconds(120),
            };
            var client = new RestClient(options);
            var login = new RestRequest("/auth/login", Method.Post);
            login.AlwaysMultipartFormData = true;
            login.AddParameter("username", username);
            login.AddParameter("password", password);
            RestResponse response = client.Execute(login);
            Console.WriteLine(response.Content);
            Console.WriteLine(response.ResponseUri);
            return response;
        }

        public RestResponse CreateClassWithSubjects(string class_name, string subject_1, string subject_2, string subject_3, string token)
        {

            if (string.IsNullOrEmpty(token))
            {
                throw new ArgumentException("Authorization token is missing!");
            }

            var options = new RestClientOptions("https://schoolprojectapi.onrender.com/")
            {
                Timeout = TimeSpan.FromSeconds(120),
            };
            var client = new RestClient(options);

            var request = new RestRequest("/classes/create", Method.Post);
            request.AddQueryParameter("class_name", class_name);
            request.AddQueryParameter("subject_1", subject_1);
            request.AddQueryParameter("subject_2", subject_2);
            request.AddQueryParameter("subject_3", subject_3);

            request.AddHeader("authorization", $"Bearer {token}");

            var response = client.Execute(request);
            Console.WriteLine(response.ResponseUri);
            Console.WriteLine(response.Content);
            return response;
        }
    }
}

