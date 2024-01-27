using Ethereal_Cloud.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.ResponseCaching;
using Newtonsoft.Json;
using NuGet.Protocol.Plugins;
using System.Text;

namespace Ethereal_Cloud.Helpers
{
    public class ApiRequest
    {
        public static async Task Login()
        {
            //maybe change this to just one method. that checks if it needs the authtoken or not
        }


        //Viewdata is only used for console logging
        public static async Task<object> Files(ViewDataDictionary viewData,HttpContext context, string endpoint, Dictionary<string, object?> bodyObject)
        {
            string apiUrl = "http://" + Environment.GetEnvironmentVariable("SC_IP") + ":8090/" + endpoint;

            using (HttpClient client = new HttpClient())
            {

                //Try to get and add auth token to request body
                string? authToken = AuthTokenManagement.GetToken(context);
                if (authToken != null)
                {
                    bodyObject.Add("authtoken", authToken);
                }

                //Convert body object to a json string
                var content = new StringContent(JsonConvert.SerializeObject(bodyObject), Encoding.UTF8, "application/json");

                //Make request
                var responseEndpoint = await client.PostAsync(apiUrl, content);

                if (responseEndpoint.IsSuccessStatusCode)
                {
                    string stringResponseData = await responseEndpoint.Content.ReadAsStringAsync();
                    Response<object> responseData = await Response<object>.DeserializeJSON(stringResponseData);

                    if (responseData.Success)
                    {
                        //Request successfull
                        return responseData.Message;
                    }
                    else
                    {
                        //Invalid request
                        Logger.LogToConsole(viewData, "Invalid: " + responseData.Message);
                        return null;
                    }
                }
                else
                {
                    //Couldn't connect to endpoint
                    Logger.LogToConsole(viewData, "Invalid: " + responseEndpoint.RequestMessage);
                    return null;
                }

            }

        }
    }
}
