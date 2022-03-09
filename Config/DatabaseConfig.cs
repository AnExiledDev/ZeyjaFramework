namespace ZeyjaFramework.Config
{
    /// <summary>
    /// The database configuration.
    /// </summary>
    public class DatabaseConfig
    {
        /// <summary>
        /// Gets or sets the database hostname or ip.
        /// </summary>
        public string Server { internal get; set; } = "127.0.0.1";

        /// <summary>
        /// Gets or sets the database port.
        /// </summary>
        public int Port { internal get; set; } = 3306;

        /// <summary>
        /// Gets or sets the database name to interact with.
        /// </summary>
        public string DatabaseName { internal get; set; } = "zeyja";

        /// <summary>
        /// Gets or sets the username for connection.
        /// </summary>
        public string Username { internal get; set; } = "root";

        /// <summary>
        /// Gets or sets the password for connection.
        /// </summary>
        public string Password { internal get; set; } = "root";

        /// <summary>
        /// Gets or sets the max total database connections.
        /// </summary>
        public int MaxConnections { internal get; set; } = 155;

        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseConfig"/> class.
        /// </summary>
        public DatabaseConfig()
        {

        }

        /// <summary>
        /// Gets the connection string from configured values.
        /// </summary>
        /// 
        /// <returns>The connection string.</returns>
        public string GetConnectionString()
        {
            return $"Host={Server};Port={Port};User={Username};Password={Password};Database={DatabaseName};SslMode=None";
        }
    }
}
