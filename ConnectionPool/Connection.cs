using ConnectionPool.Common.Base;
using ConnectionPool.Common.Interfaces;
using Oracle.ManagedDataAccess.Client;

namespace ConnectionPool
{
    public class Connection : ConnectionBase<OracleConnection, string>
    {
        
    }
}
