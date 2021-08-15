using MySql.Data.MySqlClient;
using SqlKata.Compilers;
using SqlKata.Execution;
using System.Collections.Generic;
using System.Linq;
using ZeyjaFramework.Config;

namespace ZeyjaFramework
{
    /// <summary>
    /// Handles everyday database interactions.
    /// </summary>
    public class Database
    {
        private readonly DatabaseConfig Config;

        private readonly List<QueryFactory> AvailableConnections = new();
        private readonly List<QueryFactory> ReservedConnections = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="Database"/> class.
        /// </summary>
        /// 
        /// <param name="config">The <see cref="DatabaseConfig"/>.</param>
        public Database(DatabaseConfig config = null)
        {
            Config = config;
        }

        /// <summary>
        /// Reserves a database connection <see cref="ReserveConnection"/> 
        /// or creates a new database connection <see cref="InitializeConnection"/>.
        /// </summary>
        /// 
        /// <returns>A query factory connection.</returns>
        public QueryFactory DB()
        {
            return ReserveConnection();
        }

        /// <summary>
        /// Initializes a new connection.
        /// </summary>
        /// 
        /// <param name="andReserve">if true the connection is created in <see cref="ReservedConnections"/>
        ///     opposed to <see cref="AvailableConnections"/>.</param>
        ///     
        /// <returns>A QueryFactory.</returns>
        private QueryFactory InitializeConnection(bool andReserve = false)
        {
            QueryFactory newConn = new QueryFactory(new MySqlConnection(Config.GetConnectionString()), new MySqlCompiler());

            if (andReserve) { ReservedConnections.Add(newConn); }
            else { AvailableConnections.Add(newConn); }

            return newConn;
        }

        /// <summary>
        /// Reserves an existing connection.
        /// </summary>
        /// 
        /// <returns>A QueryFactory.</returns>
        private QueryFactory ReserveConnection()
        {
            if (!AvailableConnections.Any())
            {
                CleanupConnections(); // TODO: Move to scheduled task, or better implementation

                return InitializeConnection(true);
            }

            QueryFactory newConn = AvailableConnections.First();

            AvailableConnections.Remove(newConn);
            ReservedConnections.Add(newConn);

            return newConn;
        }

        /// <summary>
        /// Releases unused connections in <see cref="ReservedConnections"/> to <see cref="AvailableConnections"/> and 
        ///     reopens any closed connections.
        /// </summary>
        private void CleanupConnections()
        {
            foreach (QueryFactory conn in ReservedConnections)
            {
                switch (conn.Connection.State)
                {
                    case System.Data.ConnectionState.Closed:
                        conn.Connection.Open();

                        ReservedConnections.Remove(conn);
                        AvailableConnections.Add(conn);
                        break;

                    case System.Data.ConnectionState.Open:
                        ReservedConnections.Remove(conn);
                        AvailableConnections.Add(conn);
                        break;

                    case System.Data.ConnectionState.Connecting:
                        break;

                    case System.Data.ConnectionState.Executing:
                        break;

                    case System.Data.ConnectionState.Fetching:
                        break;

                    case System.Data.ConnectionState.Broken:
                        conn.Connection.Open();

                        ReservedConnections.Remove(conn);
                        AvailableConnections.Add(conn);
                        break;
                }
            }
        }
    }
}
