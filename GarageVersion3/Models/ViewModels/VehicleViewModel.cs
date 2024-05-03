using System.ComponentModel.DataAnnotations;

namespace GarageVersion3.Models.ViewModels
{
    public class VehicleViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Vehicle Type")]
        public string VehicleType { get; set; }

        [Display(Name = "User")]
        public string User { get; set; }

        [Display(Name = "Registration Number")]
        [Required(ErrorMessage = "Write down the vehicle registration number")]
        [StringLength(10, MinimumLength = 6)]
        public string RegistrationNumber { get; set; }
    }
}
