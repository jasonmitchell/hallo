using System;

namespace Hallo.Serialization
{
    public class InvalidJsonSerializerOptionsException : Exception
    {
        public InvalidJsonSerializerOptionsException(string message) 
            : base(message) { }
    }
}