using System;

namespace ConnectionPool.Common.Interfaces
{
    public interface IConnection : IDisposable
    {
        void SetConnectionString(string connectionString);
        void Open();
        void Close();
    }
}
