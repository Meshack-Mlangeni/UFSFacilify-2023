using Microsoft.AspNetCore.Authentication.Cookies;

namespace UFSQQFacilities.Models
{
    public class Facility
    {
        public int FacilityId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int Space { get; set; }
        public string SecurityEmail { get; set; }
        public bool Available { get; set; } = true;

        public ICollection<FacilityImage> Images { get; set; }
        public ICollection<Category> Categories { get; set; }
        public ICollection<Booking> Bookings { get; set; }
        public ICollection<Review> Reviews{ get; set; }
    }
}
