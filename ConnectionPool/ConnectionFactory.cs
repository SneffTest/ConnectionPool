using ConnectionPool.Common.Interfaces;
using Microsoft.Extensions.Logging;
using Oracle.ManagedDataAccess.Client;
using System;

namespace ConnectionPool
{
    public class ConnectionFactory : IConnectionFactory<Connection, string, OracleConnection>
    {
        private string _connectionString;
        private readonly ILogger<ConnectionFactory> _logger;

        public ConnectionFactory(ILogger<ConnectionFactory> logger)
        {
            this._logger = logger;
        }

        public void SetConnectionString(string connectionString)
        {
            this._connectionString = connectionString;
        }

        public Connection CreateConnection()
        {
            _logger.LogInformation("CreateConnection started");
            var conn = new Connection();
            conn.SetConnectionString(_connectionString);
            conn.ConnID = GenerateKey();
            conn.Open();

            _logger.LogInformation("CreateConnection ready");
            return conn;
        }

        private string GenerateKey()
        {
            var key= Guid.NewGuid().ToString();
            _logger.LogInformation($"New key {key}");
            return key;
        }
    }
}
