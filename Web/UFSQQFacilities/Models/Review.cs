using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace UFSQQFacilities.Models
{
    public class Review
    {
        public int ReviewId { get; set; }

        [Range(0, 5, ErrorMessage = "Ratings must be between 0 to 5")]
        [DisplayName("Rating")]
        public int Rating { get; set; }

        [Required(ErrorMessage ="Please enter comment for review")]
        [DisplayName("Comment")]
        public string Comment { get; set; }

        public string UserEmail { get; set; }
        public int FacilityId { get; set; }
    }
}
