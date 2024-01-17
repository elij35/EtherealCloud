using System.Net.Http.Headers;
using System.Text.Json;

namespace StorageController.Data
{
    public class BucketAPIHandler
    {

        public static async Task<Response<string>> GetFileContent(int fileID)
        {

            string BUCK_IP = Environment.GetEnvironmentVariable("BUCK_IP");
            string BUCK_PORT = Environment.GetEnvironmentVariable("BUCK_PORT");

            Response<string> responseObj;
            using (HttpClient client = new HttpClient())
            {

                HttpResponseMessage response = await client.GetAsync($"http://{BUCK_IP}:{BUCK_PORT}/file/{fileID}");
                string responseText = await response.Content.ReadAsStringAsync();

                responseObj = await Response<string>.DeserializeJSON(responseText);

            }

            return responseObj;

        }

        private struct ContentStruct
        {
            public string FileContent { get; set; }
        }

        public static async Task<Response<string>> SendFileContent(int fileID, string fileContent)
        {

            Response<string> responseObj;
            using (HttpClient client = new HttpClient())
            {

                ContentStruct contentStruct = new ContentStruct()
                {
                    FileContent = fileContent
                };

                HttpContent content = new StringContent(JsonSerializer.Serialize(contentStruct), new MediaTypeHeaderValue("application/json"));
                HttpResponseMessage response = await client.PostAsync($"http://{Environment.GetEnvironmentVariable("BUCK_IP")}:8070/file/{fileID}", content);
                string responseText = await response.Content.ReadAsStringAsync();

                responseObj = await Response<string>.DeserializeJSON(responseText);

            }

            return responseObj;

        }

    }
}
