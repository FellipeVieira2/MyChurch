namespace MyChurch.Domain.Entities
{
    public class Address
    {
        public Address(string street, string city, string state, string zipCode, string country, string neighborhood)
        {
            Street = street;
            City = city;
            State = state;
            ZipCode = zipCode;
            Country = country;
            Neighborhood = neighborhood;
        }

        public int Id { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
        public string Country { get; set; }
        public string Neighborhood { get; set; }
    }
}
