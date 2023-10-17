using ConnectionPool.Common.Enums;
using System;

namespace ConnectionPool.Common.Exceptions
{
    public class ConnectionPoolBaseException : Exception
    {
        public virtual ErrorCodes ErrorCode => ErrorCodes.Error;

        public ConnectionPoolBaseException(string message) : base(message)
        {

        }
    }
}
