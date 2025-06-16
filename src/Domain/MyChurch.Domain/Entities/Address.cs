using System.Collections.Generic;

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
            Created = DateTime.UtcNow;
        }

        public int Id { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
        public string Country { get; set; }
        public string Complement { get; set; }
        public string Neighborhood { get; set; }
        public string? Number { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Updated { get; set; }
        public Church Church { get; set; }

        // Relacionamento com Member
        public ICollection<Member> Members { get; set; } = new List<Member>();

        public void Update(string? street, string? city, string? zipCode, string? country, string? neighborhood, string? state, string? number, string? complement)
        {
            Street = street ?? Street;
            State = state ?? State;
            City = city ?? City;
            ZipCode = zipCode ?? ZipCode;
            Country = country ?? Country;
            Neighborhood = neighborhood ?? Neighborhood;
            Updated = DateTime.UtcNow;
            Number = number ?? Number;
            Complement = complement ?? Complement;
        }
    }
}