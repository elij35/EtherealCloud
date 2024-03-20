using System.Text.Json;

namespace Ethereal_Cloud
{
    public class Response<T>
    {

        public bool Success { get; set; }
        public T Message { get; set; }

        public static async Task<Response<T>> DeserializeJSON(string JSON)
        {
            JsonSerializerOptions options = new JsonSerializerOptions();
            options.PropertyNameCaseInsensitive = true;
            return JsonSerializer.Deserialize<Response<T>>(JSON.Trim('\"').Replace("\\", ""), options);
        }

    }
}
