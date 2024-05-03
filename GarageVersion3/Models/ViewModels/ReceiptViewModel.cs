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

        [Display(Name = "Checkin Date")]
        public DateTime Checkin { get; set; }

        [Display(Name = "Checkout Date")]
        public DateTime CheckoutDate { get; set; }

        [Display(Name = "Total Hours of Parking")]
        public int TotalParkingHours { get; set; }
        public double Price { get; set; }
    }
}
