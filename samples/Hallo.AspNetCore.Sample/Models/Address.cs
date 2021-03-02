namespace Hallo.AspNetCore.Sample.Models
{
    public class Address
    {
        public int Id { get; set; }
        public int PersonId { get; set; }
        public string HouseNameNo { get; set; }
        public string Street { get; set; }
        public string TownVillage { get; set; }
        public string City { get; set; }
        public string PostCode { get; set; }
    }
}