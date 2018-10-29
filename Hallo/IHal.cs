using System.Threading.Tasks;

namespace Hallo
{
    public interface IHal
    {
        Task<HalRepresentation> RepresentationOfAsync(object resource);
    }
}