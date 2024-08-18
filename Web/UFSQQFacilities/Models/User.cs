using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace UFSQQFacilities.Models
{

    public class User: IdentityUser
    {
        [Required(ErrorMessage ="Please enter first name")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Please enter last name.")]
        public string LastName { get; set; }

        public string IdPassportNumber { get; set; }
        public DateTime DateJoined { get; set; } = DateTime.Now;
        public DateTime DateModified { get; set; }
        public string StudentStaffNumber { get; set; }
        public string MobilePassword { get; set; } = "";
    }
}
