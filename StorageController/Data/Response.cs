using System.Text.Json;

namespace StorageController.Data
{
    public class Response<T>
    {

        public bool Success { get; set; }

        public T Message { get; set; }

        public Response(bool _Success, T _Message)
        {

            Success = _Success;
            Message = _Message;

        }

        public async Task<string> Serialize()
        {
            return JsonSerializer.Serialize(this);
        }

    }
}
