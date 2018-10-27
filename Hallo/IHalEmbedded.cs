using System.Threading.Tasks;

namespace Hallo
{
    public interface IHalEmbedded<in T>
    {
        object EmbeddedFor(T resource);
    }

    public interface IHalEmbeddedAsync<in T>
    {
        Task<object> EmbeddedForAsync(T resource);
    }
}