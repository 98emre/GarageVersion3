using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GarageVersion3.Models
{
    public class ParkingLot
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey("Vehicle")]
        public int VehicleId { get; set; }
        public Vehicle Vehicle { get; set; }
        [Range(1,25, ErrorMessage = "We only have parking spots ranging from 1 - 25.")]
        public int ParkingSpot { get; set; }
        public bool AvailableParkingSpot { get; set; }
        public DateTime Checkin { get; set; }
    }
}
