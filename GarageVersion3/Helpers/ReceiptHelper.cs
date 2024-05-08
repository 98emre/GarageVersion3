using GarageVersion3.Data;
using GarageVersion3.Models;
using GarageVersion3.Models.ViewModels;
using Microsoft.AspNetCore.Http.HttpResults;

namespace GarageVersion3.Helpers
{
    public class ReceiptHelper
    {
        private readonly GarageVersion3Context _context;
        private readonly ParkingLot parkingLotToCheckout;

        public ReceiptHelper(GarageVersion3Context dbContext, ParkingLot parkingLot)
        {
            _context = dbContext;
            parkingLotToCheckout = parkingLot;
        }
        
        public ReceiptViewModel CheckoutVehicle() 
        {
            var vehicle = _context.Vehicle.Find(parkingLotToCheckout.VehicleId);
            var parkingSpot = _context.ParkingLot.Where(p => p.VehicleId == vehicle.Id).FirstOrDefault();

            parkingSpot.AvailableParkingSpot = true;
            _context.Update(parkingSpot);
            _context.SaveChanges();

            ReceiptViewModel receiptVM = new ReceiptViewModel();
            receiptVM.User = vehicle.User;
            receiptVM.VehicleType = vehicle.VehicleType;
            receiptVM.RegistrationNumber = vehicle.RegistrationNumber;
            receiptVM.Checkin = parkingSpot.Checkin;
            receiptVM.CheckoutDate = DateTime.Now;
            receiptVM.CalculateTotalParkingHours();
            receiptVM.CalculatePrice();

            Receipt receipt = new Receipt();
            receipt.User = _context.User.Where(p => p.Id == vehicle.UserId).FirstOrDefault();
            receipt.ParkingNumber = parkingSpot.ParkingSpot;
            receipt.CheckIn = parkingSpot.Checkin;
            receipt.CheckOut = DateTime.Now;
            receipt.UserId = vehicle.UserId;
            receipt.Price = receiptVM.Price;

            _context.Add(receipt);
            _context.Update(parkingSpot);
            return receiptVM;
        }
        
    }
}
