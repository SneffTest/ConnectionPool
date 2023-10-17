using ConnectionPool.Common.Interfaces;
using System.Data;

namespace ConnectionPool.Common.Base
{
    public abstract class ConnectionBase<TBaseConnection, TConnKey> : IConnection 
        where TBaseConnection : class, IDbConnection, new()
        where TConnKey : notnull
    {
        private TBaseConnection _conn;
        private string _connectionString;

        /// <summary>
        /// The actual connection identifier
        /// </summary>
        public TConnKey ConnID { get; set; }

        /// <summary>
        /// Sets the connectionstring for the current connection
        /// </summary>
        /// <param name="connectionString">The database connection string for the current conneciton</param>
        public void SetConnectionString(string connectionString)
        {
            this._connectionString = connectionString;
        }

        /// <summary>
        /// Closes the current connection
        /// </summary>
        public virtual void Close()
        {
            _conn?.Close();
        }

        /// <summary>
        /// Disposes the current object
        /// </summary>
        public virtual void Dispose()
        {
            _conn?.Close();
            _conn?.Dispose();
        }

        /// <summary>
        /// Opens a new connection
        /// </summary>
        public void Open()
        {
            _conn = new TBaseConnection();
            _conn.ConnectionString = _connectionString;
            _conn.Open();
        }
    }
}
