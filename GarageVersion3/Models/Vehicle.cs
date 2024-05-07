using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GarageVersion3.Models
{
    public class Vehicle
    {
        [Key]
        public int Id { get; set; }
        
        [ForeignKey("VehicleType")]
        public int VehicleTypeId { get; set; }

        [Required(ErrorMessage = "Add type for your vehicle")]
        public VehicleType? VehicleType { get; set; }

        [ForeignKey("User")]
        public int UserId { get; set; }

        [Required(ErrorMessage = "Add owner for the vehicle")]
        public User? User { get; set; }
        
        [Required(ErrorMessage = "Add color for your vehicle")]
        public string Color { get; set; }

        [Required(ErrorMessage = "Add brand for your vehicle")]
        public string Brand { get; set; }

        [Display(Name = "Model")]
        [Required(ErrorMessage = "Add model for your vehicle")]
        public string VehicleModel { get; set; }

        [Display(Name = "Number of Wheels")]
        [Range(0, 100)]
        [Required(ErrorMessage = "Add nr of wheels for your vehicle")]
        public int NrOfWheels { get; set; }

        [Required(ErrorMessage = "Write down the vehicle registration number")]
        [StringLength(10, MinimumLength = 6)]
        public string RegistrationNumber { get; set; }
    }
}
