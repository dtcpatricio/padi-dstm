using System;

namespace CommonTypes
{
    public class ServerFailedException : Exception
    {
        public ServerFailedException()
        {
        }

        public ServerFailedException(string message)
            : base(message)
        {
        }

        public ServerFailedException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
