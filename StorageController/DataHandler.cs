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
