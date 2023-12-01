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