using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace UFSQQFacilities.Models.ViewModels
{
    public class AddFacilityViewModel
    {
        [Required(ErrorMessage = "Please enter a unique facility name")]
        [DisplayName("Facility name")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Please enter facility description")]
        [DisplayName("Facility description")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Please enter facility space")]
        [DisplayName("Facility space")]
        public int Space { get; set; }

        [Required(ErrorMessage = "Please enter facility price in rands")]
        [DisplayName("Base price")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Please enter categories separated by comma.")]
        [DisplayName("Categories separated by comma")]
        [DataType(DataType.Text)]
        public string Categories { get; set; }

        [DisplayName("Facility image(s)")]
        public IFormFileCollection Images { get; set; }

    }
}
