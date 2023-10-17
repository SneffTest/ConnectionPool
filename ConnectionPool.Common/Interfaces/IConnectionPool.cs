using ConnectionPool.Common.Base;
using System.Data;
using System.Data.Common;

namespace ConnectionPool.Common.Interfaces
{
    public interface IConnectionPool<TConn, TConnKey, TBaseConnection>
        where TConn : ConnectionBase<TBaseConnection, TConnKey>, new()
        where TConnKey : notnull
        where TBaseConnection : class, IDbConnection, new()
    {
        void SetConnectionString(string connectionString);

        void checkIn(TConn conn);
        TConn checkOut(TConnKey connID);
        TConn checkOutNew();
        void CloseAndRemoveConn(TConn conn);
        void CloseAndRemoveConn(TConnKey connID);
        int GetConnectionCount();
    }
}