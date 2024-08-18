using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UFSQQFacilities.Models
{

    public class Booking

    {
        public int BookingId { get; set; }

        [Required]
        [DisplayName("Enter the date you need the facility")]
        public DateTime DateStart { get; set; }
        public DateTime DateEnd { get; set; }
        public string BookingPass { get; set; }

        public bool IsValid { get; set; }
        public bool Approved { get; set; }

        public string UserEmail { get; set; }

        public int FacilityId { get; set; }

        [DeleteBehavior(DeleteBehavior.NoAction)]
        public Facility Facilities { get; set; }

        [DisplayName("Select category")]
        public int CategoryId { get; set; }
        public Category Categories { get; set; }
    }
}
