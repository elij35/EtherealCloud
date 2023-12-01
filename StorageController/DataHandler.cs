using Microsoft.Data.SqlClient;

namespace StorageController
{
    public class DataHandler
    {

        public string connectionString = 
                "Data Source=localhost;" +
                "User id=SA;" +
                "Password=EtherealDatabaseStorage!!;" +
                "TrustServerCertificate=True;";

        public DataHandler()
        {
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

        /// <summary>
        /// Opens the connection, has a timeout of 5 seconds (attemps to connect each second if failed)
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