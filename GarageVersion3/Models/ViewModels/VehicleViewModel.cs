using System.ComponentModel.DataAnnotations;

namespace GarageVersion3.Models.ViewModels
{
    public class VehicleViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Vehicle Type")]
        public string? VehicleType { get; set; }
        public int VehicleTypeId { get; set; }

        [Display(Name = "User")]
        public string? User { get; set; }
        public int UserId { get; set; }

        [Display(Name = "Registration Number")]
        [Required(ErrorMessage = "Write down the vehicle registration number")]
        [StringLength(10, MinimumLength = 6)]
        public string RegistrationNumber { get; set; }

        [Required(ErrorMessage = "Add color for your vehicle")]
        public string Color { get; set; }

        [Required(ErrorMessage = "Add brand for your vehicle")]
        public string Brand { get; set; }

        [Display(Name = "Model")]
        [Required(ErrorMessage = "Add model for your vehicle")]
        public string VehicleModel { get; set; }

        [Display(Name = "Number of Wheels")]
        [Range(0, 100)]
        [Required(ErrorMessage = "Add number of wheels for your vehicle")]
        public int NrOfWheels { get; set; }

        public int MaxParkingSize { get; set; }

    }
}
