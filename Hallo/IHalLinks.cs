using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hallo
{
    public interface IHalLinks<in T>
    {
        IEnumerable<Link> LinksFor(T resource);
    }

    public interface IHalLinksAsync<in T>
    {
        Task<IEnumerable<Link>> LinksForAsync(T resource);
    }
}