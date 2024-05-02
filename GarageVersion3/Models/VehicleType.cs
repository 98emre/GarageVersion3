using System.ComponentModel.DataAnnotations;

namespace GarageVersion3.Models
{
    public class VehicleType
    {
        [Key]
        public int Id { get; set; }
        // 0.33 to allow for bikes
        public string Type { get; set; }
        [Range(0.33, 3, ErrorMessage = "Vehicle size must be between 0.33 & 3")]
        public double ParkingSize { get; set; }
    }
}