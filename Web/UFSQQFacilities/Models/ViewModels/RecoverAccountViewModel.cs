using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace UFSQQFacilities.Models.ViewModels
{
    public class RecoverAccountViewModel
    {
        public string Email { get; set; }

        [Required(ErrorMessage = "Please enter password recovery question")]
        [DisplayName("Password recovery question")]
        [DataType(DataType.Text)]
        public string SecurityQuestion { get; set; }

        [Required(ErrorMessage = "Please enter password recovery answer")]
        [DisplayName("Password recovery answer")]
        [DataType(DataType.Text)]
        public string SecurityAnswer { get; set; }

        [Required(ErrorMessage = "Please new enter password")]
        [DisplayName("Password")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Please confirm password")]
        [DisplayName("Confirm password")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Passwords must match")]
        public string ConfirmPassword { get; set; }
    }
}
