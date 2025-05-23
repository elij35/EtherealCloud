﻿using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace Ethereal_Cloud.Helpers
{
    public class ApiRequest
    {
        public static async Task<object?> Files(HttpContext context, string endpoint, Dictionary<string, object?> bodyObject)
        {
            string apiUrl = "http://" + Environment.GetEnvironmentVariable("SC_IP") + ":8090/" + endpoint;

            using (HttpClient client = new HttpClient())
            {
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
                        return null;
                    }
                }
                else
                {
                    //Couldn't connect to endpoint
                    return null;
                }

            }

        }
    }

    public class ApiRequestV2
    {
        public static async Task<object?> Files(HttpContext context, string endpoint, bool AuthTokenRequired, Dictionary<string, object?>? bodyObject)
        {
            string apiUrl = "http://" + Environment.GetEnvironmentVariable("SC_IP") + ":8090/" + endpoint;

            using (HttpClient client = new HttpClient())
            {
                // Add AuthToken to header if required
                if (AuthTokenRequired)
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", CookieManagement.GetAuthToken(context));
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
                        return null;
                    }
                }
                else
                {
                    //Couldn't connect to endpoint
                    return null;
                }

            }

        }
    }

}

