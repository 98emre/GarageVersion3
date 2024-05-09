using System.ComponentModel.DataAnnotations;

namespace GarageVersion3.Models.ViewModels
{
    public class ReceiptViewModel
    {
        public int Id { get; set; }
        public User User { get; set; }
        
        [Display(Name = "Vehicle Type")]
        public VehicleType VehicleType { get; set; }

        [Display(Name = "Registration Number")]
        public string RegistrationNumber { get; set; }

        [Display(Name = "Parking Spot")]
        public int ParkingNumber { get; set; }

        [Display(Name = "Check in Date")]
        public DateTime CheckIn { get; set; }

        [Display(Name = "Check out Date")]
        public DateTime CheckOutDate { get; set; }

        [Display(Name = "Total Hours of Parking")]
        public int TotalParkingHours { get; set; }

        [DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString = "{0:N} SEK")]
        public double Price { get; set; }

        public void CalculateTotalParkingHours()
        {
            TimeSpan parkingDuration = CheckOutDate - CheckIn;
            TotalParkingHours = (int)parkingDuration.TotalHours;
        }

        public void CalculatePrice()
        {
            const double HourlyRate = 50;
            Price = TotalParkingHours * HourlyRate;
        }
    }
}
