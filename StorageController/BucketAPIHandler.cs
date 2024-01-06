using StorageController.Data;
using System.Net.Http.Headers;
using System.Text.Json;

namespace StorageController
{
    public class BucketAPIHandler
    {

        public static async Task<Response<string>> GetFileContent(int fileID)
        {

            Response<string> responseObj;
            using(HttpClient client = new HttpClient())
            {

                HttpResponseMessage response = await client.GetAsync($"http://{Environment.GetEnvironmentVariable("BUCK_IP")}:8070/file/{fileID}");
                string responseText = await response.Content.ReadAsStringAsync();

                responseObj = await Response<string>.DeserializeJSON(responseText);

            }

            return responseObj;

        }


    }
}
