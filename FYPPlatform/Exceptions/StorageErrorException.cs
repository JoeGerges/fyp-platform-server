using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FYPPlatform.Web.Exceptions
{
    public class StorageErrorException: Exception
    {
        public StorageErrorException()
        {
        }

        public StorageErrorException(string message, Exception innerException, int statusCode) : base(message, innerException)
        {
            StatusCode = statusCode;
        }

        public StorageErrorException(string message, int statusCode) : base(message)
        {
            StatusCode = statusCode;
        }

        public StorageErrorException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public int StatusCode { get; }
    }
}
