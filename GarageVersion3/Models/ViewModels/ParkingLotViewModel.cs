namespace GarageVersion3.Models.ViewModels
{
    public class ParkingLotViewModel
    {
      public int Id { get; set; }
      //User
      public User User { get; set; }
      //Membership
      //?????????????????

      //VehicleType
      public VehicleType VehicleType { get; set; }
      //RegistrationNumber
      public Vehicle RegistrationNumber { get; set; }

      //ParkingHours
     // public ReceiptViewModel TotalParkingHours { get; set; }
    }
}
