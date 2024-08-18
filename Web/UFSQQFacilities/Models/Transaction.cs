using System.ComponentModel.DataAnnotations;

namespace UFSQQFacilities.Models
{
    public class Transaction
    {
        public int Id { get; set; }
        [Required]
        public DateTime Date { get; set; }
        [Required]
        public bool Success { get; set; }

        [Required]
        public string UserEmail { get; set; }

        public int BookingId { get; set; }
        public Booking Booking { get; set; }
    }
}
