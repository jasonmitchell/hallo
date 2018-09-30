using System.IO;
using System.Linq;
using Hal.Poc.Models;
using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json;

namespace Hal.Poc.Data
{
    public class PeopleRepository
    {
        private readonly string _dataPath;
        private Person[] _people;

        public PeopleRepository(IHostingEnvironment hostingEnvironment)
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
            _people = JsonConvert.DeserializeObject<Person[]>(json);
        }

        public Person[] List()
        {
            EnsureDataLoaded();
            return _people;
        }

        public Person Get(int id)
        {
            EnsureDataLoaded();
            return _people.SingleOrDefault(x => x.Id == id);
        }
    }
}