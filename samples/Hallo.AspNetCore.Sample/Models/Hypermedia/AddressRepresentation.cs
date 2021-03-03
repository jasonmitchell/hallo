using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace Hallo.AspNetCore.Sample.Models.Hypermedia
{
    public class AddressRepresentation : Hal<Address>,
        IHalLinks<Address>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AddressRepresentation(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }
        public IEnumerable<Link> LinksFor(Address resource)
        {
            yield return new Link(Link.Self, $"{_httpContextAccessor.HttpContext?.Request.Path}/{resource.Id}");
        }
    }
    
    public class AddressListRepresentation : PagedListRepresentation<Address>
    {
        public AddressListRepresentation(AddressRepresentation addressRepresentation, IHttpContextAccessor httpContextAccessor) 
            : base(httpContextAccessor.HttpContext?.Request.Path, addressRepresentation) { }
    }
}