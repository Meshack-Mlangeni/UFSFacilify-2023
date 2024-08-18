using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace UFSQQFacilities.Models.ViewModels
{
    public class LoginViewModel
    {
        [Required]
        [UIHint("email")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [UIHint("password")]
        public string Password { get; set; }
        public bool RememberMe { get; set; }
        public string ReturnUrl { get; set; } = "/";
    }

    public class RegisterViewModel
    {
        public string RegisterAs { get; set; } = "studentstaff";

        [Required(ErrorMessage = "Please enter a unique email address")]
        [DisplayName("Email address")]
        [DataType(DataType.EmailAddress)]
        public string EmailAddress { get; set; }

        [Required(ErrorMessage = "Please enter ID or passport number.")]
        [DisplayName("ID or Passport number")]
        public string IdPassportNumber { get; set; }

        [DisplayName("Student or Staff number")]
        public string StudentStaffNumber { get; set; }


        [Required(ErrorMessage = "Please enter first name")]
        [DisplayName("First name")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Please enter last name")]
        [DisplayName("Last name")]
        [DataType(DataType.Text)]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Please enter password")]
        [DisplayName("Password")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Please confirm password")]
        [DisplayName("Confirm password")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Passwords must match")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Please enter password recovery question")]
        [DisplayName("Password recovery question")]
        [DataType(DataType.Text)]
        public string SecurityQuestion { get; set; }

        [Required(ErrorMessage = "Please enter password recovery answer")]
        [DisplayName("Password recovery answer")]
        [DataType(DataType.Text)]
        public string SecurityAnswer { get; set; }

    }
    public class EditViewModel
    {
        public string Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }

        [DisplayName("ID or Passport number")]
        public string IdPassportNumber { get; set; }

        [DisplayName("Student or Staff number")]
        public string StudentStaffNumber { get; set; }


        [Required(ErrorMessage = "Please enter first name")]
        [DisplayName("First name")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Please enter last name")]
        [DisplayName("Last name")]
        [DataType(DataType.Text)]
        public string LastName { get; set; }

        [DisplayName("Password")]
        [DataType(DataType.Password)]
        public string Password { get; set; }


        [DisplayName("Confirm password")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Passwords must match")]
        public string ConfirmPassword { get; set; }
    }
}
