using System.Text.Encodings.Web;
using System.Text.Json;

namespace Hallo.Serialization
{
    public static class HalJsonSerializer
    {
        public static readonly JsonSerializerOptions DefaultSerializerOptions = new()
        {
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };
    }
}