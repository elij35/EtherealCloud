using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using StorageController.Data;
using System.Data;

namespace StorageController.Controllers
{

    [ApiController]
    [Route("/file")]
    public class FileController : Controller
    {

        public struct FileData
        {
            public string Filename { get; set; }
            public string Filetype { get; set; }
            public string Content { get; set; }
        }

        public struct FileDataSave
        {
            public string Filename { get; set; }
            public string Filetype { get; set; }
            public string Content { get; set; }
        }

        public struct FileRequest
        {
            public string AuthToken { get; set; }
            public string FileID { get; set; }
        }

        [HttpGet]
        [Route("/file")]
        [Consumes("application/json")]
        [Produces("application/json")]
        public async Task<string> GetFile([FromBody] FileRequest fileRequest)
        {

            string sql_Get_File = "SELECT FileID, FileName, FileType FROM ethereal.Files WHERE FileID = @FileID;";

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@FileID", fileRequest.FileID)
            };

            DataHandler db = DataHandler.instance;
            DataTable dataTable = await db.ParametizedQuery(sql_Get_File, parameters);
            Dictionary<string, string[]>? entries = await db.DataTableToDictionary(dataTable);

            if (entries == null)
                return await new Response<string>(false, "Invalid file").Serialize();

            Response<string> fileContent = await BucketAPIHandler.GetFileContent(int.Parse(entries["FileID"][0]));

            if (!fileContent.Success)
                return await new Response<string>(false, "Cannot find file contents").Serialize();

            FileData fileData = new FileData
            {
                Filename = entries["FileName"][0],
                Filetype = entries["FileType"][0],
                Content = fileContent.Message
            };

            return await new Response<FileData>(true, fileData).Serialize();
        }

        public struct FileSaveInfo
        {
            public string Message { get; set; }
            public int FileID { get; set; }
        }

        [HttpPost]
        [Route("/file")]
        [Consumes("application/json")]
        [Produces("application/json")]
        public async Task<string> SaveFile([FromBody] FileDataSave fileData)
        {

            string sql_Save_File = "INSERT INTO ethereal.Files (FileName, FileType, Folder) VALUES (@FileName, @FileType, 0)";

            SqlParameter[] parameters =
            {
                new SqlParameter("@FileName", fileData.Filename),
                new SqlParameter("@FileType", fileData.Filetype)
            };

            DataHandler db = DataHandler.instance;
            DataTable rows = await db.StaticQuery("SELECT FileName FROM ethereal.Files");

            int nextID = rows.Rows.Count + 1;
            Response<string> savedFile = (await BucketAPIHandler.SendFileContent(nextID, fileData.Content));

            int success = 0;
            if (savedFile.Success)
                success = await db.ParametizedNonQuery(sql_Save_File, parameters);

            if (success < 1)
                return await new Response<string>(false, "Could not save file.").Serialize();

            FileSaveInfo fileSaveInfo = new FileSaveInfo()
            {
                Message = "File Saved.",
                FileID = nextID
            };
            return await new Response<FileSaveInfo>(true, fileSaveInfo).Serialize();
        }
    }
}
