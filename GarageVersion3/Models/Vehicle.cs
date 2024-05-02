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
        public VehicleType VehicleType { get; set; }
        [ForeignKey("User")]
        public int UserId { get; set; }
        public User User { get; set; }
        public int RegistrationNumber { get; set; }
    }
}
