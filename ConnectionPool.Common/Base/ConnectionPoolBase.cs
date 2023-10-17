using ConnectionPool.Common.Exceptions;
using ConnectionPool.Common.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Data;

namespace ConnectionPool.Common.Base
{
    public abstract class ConnectionPoolBase<TConn, TConnKey, TBaseConnection> : IConnectionPool<TConn, TConnKey, TBaseConnection>, IDisposable
        where TConn : ConnectionBase<TBaseConnection, TConnKey>, new()
        where TConnKey : notnull
        where TBaseConnection : class, IDbConnection, new()
    {
        private string _connectionString;
        private readonly ILogger _logger;
        private readonly IConnectionFactory<TConn, TConnKey, TBaseConnection> _factory;
        private ConcurrentDictionary<TConnKey, TConn> _connections = new ConcurrentDictionary<TConnKey, TConn>();

        public ConnectionPoolBase(IConnectionFactory<TConn, TConnKey, TBaseConnection> factory, ILogger logger)
        {
            this._factory = factory;
            this._logger = logger;
        }

        /// <summary>
        /// Sets the connectionstring for the connections, which are opened by the conneciton pool
        /// </summary>
        /// <param name="connectionString">The connection string</param>
        public void SetConnectionString(string connectionString)
        {
            var currentConnectionKeys = _connections.Keys;
            this._connectionString = connectionString;
            foreach (var connKey in currentConnectionKeys)
            {
                CloseAndRemoveConn(connKey);
            }
        }

        /// <summary>
        /// Returns the connection to the connection pool
        /// </summary>
        /// <param name="conn">The connection which should be returned</param>
        /// <exception cref="ConnectionPoolNotFoundException">If connection was never opened</exception>
        public virtual void checkIn(TConn conn)
        {
            _logger.LogInformation($"checkIn for {conn.ConnID} started");

            if (_connections.TryGetValue(conn.ConnID, out var origConn))
            {
                origConn = conn;
            }
            else
            {
                _logger.LogError($"checkIn: connection not found connID {conn.ConnID}");
                throw new ConnectionPoolNotFoundException(conn.ConnID.ToString());
            }

            _logger.LogInformation($"checkIn for {conn.ConnID} ready");
        }

        /// <summary>
        /// Gets a connection from the connectionpool
        /// </summary>
        /// <param name="connID">The connection identifier</param>
        /// <returns>The connection corresponding with the given key</returns>
        /// <exception cref="ConnectionPoolNotFoundException">If conneciton was not found</exception>
        public virtual TConn checkOut(TConnKey connID)
        {
            _logger.LogInformation($"checkOut for {connID} started");

            TConn conn;
            if (!_connections.TryGetValue(connID, out conn))
            {
                _logger.LogError($"checkOut: connection not found connID {connID}");
                throw new ConnectionPoolNotFoundException(connID.ToString());
            }

            _logger.LogInformation($"checkOut for {conn.ConnID} ready");
            return conn;
        }

        /// <summary>
        /// Opens a new conneciton
        /// </summary>
        /// <returns></returns>
        /// <exception cref="ConnectionPoolBaseException"></exception>
        public virtual TConn checkOutNew()
        {
            _logger.LogInformation("checkOutNew started");

            var conn = _factory.CreateConnection();
            if(_connections.TryAdd(conn.ConnID, conn))
            {
                _logger.LogInformation("checkOutNew ready");
                return conn;
            }

            _logger.LogError($"checkOutNew error! connID {conn.ConnID}");
            conn.Dispose();
            throw new ConnectionPoolBaseException("Can not open connection!");
        }

        /// <summary>
        /// Closes and removes the given conneciton
        /// </summary>
        /// <param name="connID">The connection key</param>
        public virtual void CloseAndRemoveConn(TConnKey connID)
        {
            _logger.LogInformation($"CloseAndRemoveConn started for {connID}");
            if (_connections.TryRemove(connID, out var conn))
            {
                conn.Close();
                conn.Dispose();
            }
            _logger.LogInformation($"CloseAndRemoveConn ready for {connID}");
        }

        /// <summary>
        /// Closes and removes the given conneciton
        /// </summary>
        /// <param name="conn">The connection</param>
        public void CloseAndRemoveConn(TConn conn)
        {
            CloseAndRemoveConn(conn.ConnID);
        }

        /// <summary>
        /// Gets the current conneciton count
        /// </summary>
        /// <returns>Current connection count</returns>
        public int GetConnectionCount()
        {
            return _connections.Count;
        }

        public void Dispose()
        {
            var currentConnectionKeys = _connections.Keys;
            foreach (var connKey in currentConnectionKeys)
            {
                CloseAndRemoveConn(connKey);
            }
        }
    }
}
