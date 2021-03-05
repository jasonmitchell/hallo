using System.Collections.Generic;

namespace Hallo.Test.Serialization.Supporting
{
    internal class PagedList<T>
    {
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int TotalItems { get; set; }
        public IEnumerable<T> Items { get; set; }
    }
}