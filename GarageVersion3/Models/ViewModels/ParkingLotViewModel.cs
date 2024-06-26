﻿using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace GarageVersion3.Models.ViewModels
{
    public class ParkingLotViewModel
    {
        public int Id { get; set; }
        
        public int UserId { get; set; }

        public int VehicleId { get; set; }
        
        public string? User { get; set; }

        [Display(Name = "Registration Number")]
        public string? RegistrationNumber { get; set; }

     
        public DateTime Checkin { get; set; }

        [Display(Name = "Parking Spot")]
        public int ParkingSpot { get; set; }

        public VehicleViewModel? VehicleViewModel { get; set; }

    }
}
