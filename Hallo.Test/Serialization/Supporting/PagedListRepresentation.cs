using System.Collections.Generic;
using System.Linq;

namespace Hallo.Test.Serialization.Supporting
{
    internal abstract class PagedListRepresentation<TItem> : Hal<PagedList<TItem>>, IHalState<PagedList<TItem>>,
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
}