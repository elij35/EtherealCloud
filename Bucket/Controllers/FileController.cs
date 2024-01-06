using Bucket.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Storage;
using System.Buffers;
using System.Security.Cryptography;
using System.Text;

namespace Bucket.Controllers
{

    [ApiController]
    [Route("/file/{id}")]
    public class FileController
    {

        public struct FileStruct
        {
            public string FileContent { get; set; }

        }

        [HttpPost]
        [Consumes("application/json")]
        [Produces("application/json")]
        public async Task<string> UploadFile([FromBody] FileStruct fileData, [FromRoute] int id) 
        {

            string filePath = Program.ROOT_FILE_DIR + await FileUtils.RandomFileName();
            FileStream fileStream = File.Create(filePath);

            if (!await FileUtils.WriteFile(fileStream, fileData.FileContent))
                return await new Response<string>(false, "File could not be saved.").Serialize();

            fileStream.Close();

            DatabaseContext database = new();

            if (database.FileData.Where(file => file.FileID == id).Count() > 0)
                return await new Response<string>(false, "File ID already exists").Serialize();

            await database.AddAsync(new FileData()
            {
                FileID = id,
                FilePath = filePath
            });
            await database.SaveChangesAsync();

            return await new Response<string>(true, "File successfully uploaded.").Serialize();

        }


        

    }
}
