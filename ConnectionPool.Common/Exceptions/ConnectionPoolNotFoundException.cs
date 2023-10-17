using ConnectionPool.Common.Enums;

namespace ConnectionPool.Common.Exceptions
{

    public class ConnectionPoolNotFoundException : ConnectionPoolBaseException
    {
        public override ErrorCodes ErrorCode => ErrorCodes.ConnectionNotFound;

        public ConnectionPoolNotFoundException(string key) : base($"{key} not found.")
        {
        }
    }
}
