using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hallo.Test.Integration.TestApi
{
    public class PersonRepresentation : Hal<Person>,
                                        IHalLinks<Person>,
                                        IHalEmbeddedAsync<Person>
    {
        private readonly ContactLookup _contacts;

        public PersonRepresentation(ContactLookup contacts)
        {
            _contacts = contacts;
        }
    
        public IEnumerable<Link> LinksFor(Person resource)
        {
            if (resource.Id != 404)
            {
                yield return new Link("self", $"/people/{resource.Id}");
            }
        }

        public async Task<object> EmbeddedForAsync(Person resource)
        {
            return new
            {
                contacts = await _contacts.For(resource)
            };
        }
    }

    public abstract class PagedListRepresentation<TItem> : Hal<PagedList<TItem>>, IHalState<PagedList<TItem>>,
                                                           IHalEmbedded<PagedList<TItem>>, IHalLinks<PagedList<TItem>>
    {
        private readonly string _baseUrl;
        private readonly IHalLinks<TItem> _itemLinks;

        protected PagedListRepresentation(string baseUrl, IHalLinks<TItem> itemLinks)
        {
            _baseUrl = baseUrl;
            _itemLinks = itemLinks;
        }
        
        public object StateFor(PagedList<TItem> resource)
        {
            return new
            {
                resource.CurrentPage,
                resource.TotalItems,
                resource.TotalPages
            };
        }

        public object EmbeddedFor(PagedList<TItem> resource)
        {
            var items = from item in resource.Items
                        let links = _itemLinks.LinksFor(item)
                        select new HalRepresentation(item, links);

            return new
            {
                Items = items.ToList()
            };
        }

        public IEnumerable<Link> LinksFor(PagedList<TItem> resource)
        {
            yield return new Link("self", $"{_baseUrl}?page={resource.CurrentPage}");
            yield return new Link("first", $"{_baseUrl}?page=1");
            yield return new Link("last", $"{_baseUrl}?page={resource.TotalPages}");

            if (resource.CurrentPage > 1)
            {
                yield return new Link("prev", $"{_baseUrl}?page={resource.CurrentPage - 1}");
            }

            if (resource.CurrentPage < resource.TotalPages)
            {
                yield return new Link("next", $"{_baseUrl}?page={resource.CurrentPage + 1}");
            }
        }
    }

    public class PersonListRepresentation : PagedListRepresentation<Person>
    {
        public PersonListRepresentation(PersonRepresentation personRepresentation) 
            : base("/people", personRepresentation) { }
    }
}