using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using Hallo.AspNetCore.Mvc.Sample.Models;
using Microsoft.Extensions.Hosting;

namespace Hallo.AspNetCore.Mvc.Sample.Data
{
    public class PeopleRepository
    {
        private readonly string _dataPath;
        private Person[] _people;

        public PeopleRepository(IHostEnvironment hostingEnvironment)
        {
            _dataPath = Path.Combine(hostingEnvironment.ContentRootPath, "Data/people.json");
        }
                
        private void EnsureDataLoaded()
        {
            if (_people != null)
            {
                return;
            }
            
            var json = File.ReadAllText(_dataPath);
            _people = JsonSerializer.Deserialize<Person[]>(json);
        }

        public PagedList<Person> List(Paging paging)
        {
            EnsureDataLoaded();

            var items = _people.Skip((paging.Page - 1) * paging.PageSize)
                .Take(paging.PageSize)
                .ToArray();
            
            return new PagedList<Person>
            {
                CurrentPage = paging.Page,
                TotalItems = _people.Length,
                TotalPages = (int)Math.Ceiling(_people.Length / (double)paging.PageSize),
                Items = items
            };
        }

        public Person Get(int id)
        {
            EnsureDataLoaded();
            return _people.SingleOrDefault(x => x.Id == id);
        }
    }
}