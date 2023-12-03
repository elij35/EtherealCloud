using Microsoft.Data.SqlClient;

namespace StorageController
{
    public class DataHandler
    {

        public string connectionString = 
                "User id=SA;" +
                "TrustServerCertificate=True;";

        public DataHandler(string DB_IP, string DB_PASS)
        {

            connectionString += 
                $"Data Source={DB_IP};" +
                $"Password={DB_PASS};";

            CreateDatabase();
            CreateTablesAsync();
        }

        private SqlConnection CreateConnection()
        {

            return new SqlConnection(connectionString);
            
        }

        private async void CreateDatabase()
        {

            // making sure the database exists
            await StaticQuery(
                "IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'ethereal_storage') BEGIN " +
                "CREATE DATABASE ethereal_storage;" +
                "END;"
            );

            connectionString += "Initial catalog=ethereal_storage;";

        }

        private async void CreateTablesAsync()
        {

            // make sure the schema exists
            await StaticQuery(
                "IF NOT EXISTS (SELECT name FROM sys.schemas WHERE name = N'ethereal') BEGIN " +
                "EXEC('CREATE SCHEMA ethereal;');" +
                "END;"
            );

            await StaticQuery(
                "IF OBJECT_ID(N'[ethereal].[Users]', N'U') IS NULL " +
                "CREATE TABLE [ethereal].[Users] (" +
                "[UserID] INT IDENTITY (1, 1) NOT NULL, " +
                "[Username] VARCHAR (64) NOT NULL, " +
                "[Email] VARCHAR (64) NOT NULL, " +
                "[Password] VARCHAR (64) NOT NULL, " +
                "[Administrator] BIT NOT NULL, " +
                "PRIMARY KEY CLUSTERED ([UserID] ASC)" +
                ");"
            );

            await StaticQuery(
                "IF OBJECT_ID(N'[ethereal].[Files]', N'U') IS NULL " +
                "CREATE TABLE [ethereal].[Files] (" +
                "[FileID] INT IDENTITY (1, 1) NOT NULL," +
                "[Folder] BIT NOT NULL," +
                "[FileType] VARCHAR (20) NULL," +
                "[FileName] VARCHAR (32) NOT NULL," +
                "[FilePassword] VARCHAR (64) NULL," +
                "[FileRoute] INT NULL," +
                "[Location] VARCHAR (8) NULL," +
                "[CreationDate] DATE NULL," +
                "[RedundantID] INT NULL," +
                "PRIMARY KEY CLUSTERED ([FileID] ASC)" +
                ");"
            );

            await StaticQuery(
                "IF OBJECT_ID(N'[ethereal].[UserFiles]', N'U') IS NULL " +
                "CREATE TABLE [ethereal].[UserFiles] (" +
                "[FileID] INT NOT NULL," +
                "[UserID] INT NOT NULL," +
                "[Privilege] VARCHAR (10) NOT NULL," +
                "CONSTRAINT [Link_PK] PRIMARY KEY CLUSTERED ([FileID] ASC, [UserID] ASC)," +
                "CONSTRAINT [File_FK] FOREIGN KEY ([FileID]) REFERENCES [ethereal].[Files] ([FileID]) ON DELETE CASCADE," +
                "CONSTRAINT [User_FK] FOREIGN KEY ([UserID]) REFERENCES [ethereal].[Users] ([UserID]) ON DELETE CASCADE" +
                ");"
            );

        }

        /// <summary>
        /// This is used for queries that do not return data and do not have any parameters.
        /// DO NOT INSERT USER GENERATED STRINGS INTO THE SQL TO USE THIS METHOD.
        /// Ie. DO NOT PUT "SELECT * FROM ethereal.Users WHERE UserID = " + userInput;
        /// Doing that would allow SQL injection, use the methods created specificially for queries with parameters.
        /// </summary>
        /// <param name="sql_query">The SQL statement to run.</param>
        public async Task<int> StaticQuery(string sql_query)
        {

            using ( SqlConnection connection = CreateConnection() )
            {

                OpenSQLConnection(connection);

                SqlCommand command = connection.CreateCommand();
                command.CommandText = sql_query;
                
                int rows_affected = command.ExecuteNonQuery();

                return rows_affected;

            }

        }

        /// <summary>
        /// Opens the connection, has a timeout of 10 seconds (attemps to connect each second if failed)
        /// </summary>
        /// <param name="connection">The connection to open</param>
        private void OpenSQLConnection(SqlConnection connection)
        {

            int attempts = 10;

            while (attempts > 0)
            {
                try
                {
                    connection.Open();
                    return;
                }
                catch
                {
                    Thread.Sleep(1000);

                    // Exiting if there is no attempts left as the controller could not connect to the database
                    if (attempts == 0)
                        Environment.Exit(1);

                    attempts--;
                }
            }
        }
    }
}