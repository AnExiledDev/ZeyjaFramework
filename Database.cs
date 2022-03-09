using MySql.Data.MySqlClient;
using SqlKata.Compilers;
using SqlKata.Execution;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using ZeyjaFramework.Config;
using ZeyjaFramework.Interfaces;

namespace ZeyjaFramework
{
    /// <summary>
    /// Handles everyday database interactions.
    /// </summary>
    public class Database : IDatabase
    {
        private readonly DatabaseConfig _config;

        private readonly List<QueryFactory> _availableConnections = new();
        private readonly List<QueryFactory> _reservedConnections = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="Database"/> class.
        /// </summary>
        /// 
        /// <param name="config">The <see cref="DatabaseConfig"/>.</param>
        public Database(DatabaseConfig config)
        {
            _config = config;

            for (int i = 0; i < 10; i++)
            {
                InitializeConnection();
            }
        }

        /// <summary>
        /// Reserves a database connection <see cref="ReserveConnection"/> 
        /// or creates a new database connection <see cref="InitializeConnection"/>.
        /// </summary>
        /// 
        /// <returns>A query factory connection.</returns>
        public QueryFactory Db()
        {
            return ReserveConnection();
        }

        /// <summary>
        /// Initializes a new connection.
        /// </summary>
        /// 
        /// <param name="andReserve">if true the connection is created in <see cref="_reservedConnections"/>
        ///     opposed to <see cref="_availableConnections"/>.</param>
        ///     
        /// <returns>A QueryFactory.</returns>
        private QueryFactory InitializeConnection(bool andReserve = false)
        {
            if ((_availableConnections.Count + _reservedConnections.Count) >= _config.MaxConnections)
            {
                throw new Exception("Reached max database connections.");
            }

            QueryFactory newConn = null;

            try
            {
                newConn = new QueryFactory(new MySqlConnection(_config.GetConnectionString()), new MySqlCompiler());
            }
            catch (Exception ex)
            {
                throw new Exception("Unable to connect to database.", ex);
            }

            if (andReserve) { _reservedConnections.Add(newConn); }
            else { _availableConnections.Add(newConn); }

            return newConn;
        }

        /// <summary>
        /// Reserves an existing connection.
        /// </summary>
        /// 
        /// <returns>A QueryFactory.</returns>
        private QueryFactory ReserveConnection()
        {
            if (!_availableConnections.Any())
            {
                CleanupConnections(); // TODO: Move to scheduled task, or better implementation

                return InitializeConnection(true);
            }

            QueryFactory newConn = _availableConnections.First();

            if (newConn.Connection.State == ConnectionState.Closed)
            {
                newConn.Connection.Open();
            }

            _availableConnections.Remove(newConn);
            _reservedConnections.Add(newConn);

            return newConn;
        }

        /// <summary>
        /// Releases unused connections in <see cref="_reservedConnections"/> to <see cref="_availableConnections"/> and 
        ///     reopens any closed connections.
        /// </summary>
        private void CleanupConnections()
        {
            foreach (QueryFactory conn in _reservedConnections)
            {
                switch (conn.Connection.State)
                {
                    case ConnectionState.Closed:
                        conn.Connection.Open();

                        _reservedConnections.Remove(conn);
                        _availableConnections.Add(conn);
                        break;

                    case ConnectionState.Open:
                        _reservedConnections.Remove(conn);
                        _availableConnections.Add(conn);
                        break;

                    case ConnectionState.Connecting:
                        break;

                    case ConnectionState.Executing:
                        break;

                    case ConnectionState.Fetching:
                        break;

                    case ConnectionState.Broken:
                        conn.Connection.Open();

                        _reservedConnections.Remove(conn);
                        _availableConnections.Add(conn);
                        break;
                }
            }
        }
    }
}
