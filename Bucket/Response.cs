using System.Text.Json;

namespace Bucket
{
    public class Response<T>
    {

        public bool Success { get; set; }
        public T Message { get; set; }

        public Response(bool success, T message) {  Success = success; Message = message; }

        public async Task<string>Serialize()
        {
            return JsonSerializer.Serialize(this);
        }

    }
}
