using System.Threading.Tasks;

namespace Hallo
{
    public interface IHalState<in T>
    {
        object StateFor(T resource);
    }
    
    public interface IHalStateAsync<in T>
    {
        Task<object> StateForAsync(T resource);
    }
}