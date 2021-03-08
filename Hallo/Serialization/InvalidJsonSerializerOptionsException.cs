using System;
using System.Text.Json;

namespace Hallo.Serialization
{
    /// <summary>
    /// Thrown when <see cref="JsonSerializerOptions"/> cannot be used for serializing to hal+json
    /// </summary>
    public class InvalidJsonSerializerOptionsException : Exception
    {
        public InvalidJsonSerializerOptionsException(string message) 
            : base(message) { }
    }
}