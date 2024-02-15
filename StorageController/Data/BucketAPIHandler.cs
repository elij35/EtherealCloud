using StorageController.Data.Models;
using System.Net.Http.Headers;
using System.Text.Json;

namespace StorageController.Data
{
    public class BucketAPIHandler
    {

        public static async Task<Response<string>> DeleteFileContent(int fileID, Bucket bucket)
        {

            Response<string> responseObj;
            using (HttpClient client = new HttpClient())
            {

                HttpResponseMessage response = await client.DeleteAsync($"http://{bucket.BucketIP}:{bucket.BucketPort}/file/{fileID}");
                string responseText = await response.Content.ReadAsStringAsync();

                responseObj = await Response<string>.DeserializeJSON(responseText);

            }

            return responseObj;

        }

        public static async Task<Response<string>> GetFileContent(int fileID, Bucket bucket)
        {

            Response<string> responseObj;
            using (HttpClient client = new HttpClient())
            {

                HttpResponseMessage response = await client.GetAsync($"http://{bucket.BucketIP}:{bucket.BucketPort}/file/{fileID}");
                string responseText = await response.Content.ReadAsStringAsync();

                responseObj = await Response<string>.DeserializeJSON(responseText);

            }

            return responseObj;

        }

        private struct ContentStruct
        {
            public string FileContent { get; set; }
        }

        public static async Task<Response<string>> SendFileContent(int fileID, string fileContent, Bucket bucket)
        {

            Response<string> responseObj;
            using (HttpClient client = new HttpClient())
            {

                ContentStruct contentStruct = new ContentStruct()
                {
                    FileContent = fileContent
                };

                HttpContent content = new StringContent(JsonSerializer.Serialize(contentStruct), new MediaTypeHeaderValue("application/json"));
                HttpResponseMessage response = await client.PostAsync($"http://{bucket.BucketIP}:{bucket.BucketPort}/file/{fileID}", content);
                string responseText = await response.Content.ReadAsStringAsync();

                responseObj = await Response<string>.DeserializeJSON(responseText);

            }

            return responseObj;

        }

    }
}
