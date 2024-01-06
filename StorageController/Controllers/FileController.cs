using Microsoft.AspNetCore.Mvc;
using StorageController.Data;

namespace StorageController.Controllers
{

    [Route("/file")]
    public class FileController : Controller
    {

        public struct FileData
        {
            public string Filename { get; set; }
            public string Author { get; set; }
            public string Filetype { get; set; }
            public string Content { get; set; }
        }

        public struct AuthStruct
        {
            public string Username { get; set; }
        }

        [HttpPost]
        [Consumes("application/json")]
        [Produces("application/json")]
        public async Task<string> GetFile([FromBody] AuthStruct authStruct)
        {

            FileData dummyFile = new FileData()
            {
                Filename = $"{authStruct.Username}'s File",
                Author = $"{authStruct.Username}",
                Filetype = "text",
                Content = $"This is a test text file! Author: {authStruct.Username}"
            };

            return await new Response<FileData>(true, dummyFile).Serialize();
        }
    }
}
