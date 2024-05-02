using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GarageVersion3.Models
{
    public class Receipt
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey("User")]
        public int UserId { get; set; }
        public User User { get; set; }

        [Range(1,int.MaxValue,ErrorMessage = "No negative value")]
        public int ParkingSpot { get; set; }
        public DateTime CheckIn { get; set; }
        public DateTime CheckOut { get; set; }

        [Range(0.0, double.MaxValue, ErrorMessage = "Price can't be less than 0")]
        public double Price { get; set; }


    }
}
