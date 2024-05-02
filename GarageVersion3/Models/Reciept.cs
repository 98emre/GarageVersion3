using System.ComponentModel.DataAnnotations;

namespace GarageVersion3.Models
{
    public class Reciept
    {
        [Key]
        public int Id { get; set; }
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
