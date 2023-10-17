using ConnectionPool.Common.Base;
using System.Data;

namespace ConnectionPool.Common.Interfaces
{
    public interface IConnectionFactory<TConn, TConnKey, TBaseConnection>
        where TConn : ConnectionBase<TBaseConnection, TConnKey>, new()
        where TConnKey : notnull
        where TBaseConnection : class, IDbConnection, new()
    {
        void SetConnectionString(string connectionString);
        TConn CreateConnection();
    }
}
