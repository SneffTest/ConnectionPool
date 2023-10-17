using ConnectionPool.Common.Base;
using ConnectionPool.Common.Interfaces;
using Microsoft.Extensions.Logging;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Drawing;

namespace ConnectionPool
{
    public class ConnectionPool : ConnectionPoolBase<Connection, string, OracleConnection>
    {
        public ConnectionPool(IConnectionFactory<Connection, string, OracleConnection> factory, ILogger<ConnectionPool> logger) : base(factory, logger)
        {
        }
    }
}
