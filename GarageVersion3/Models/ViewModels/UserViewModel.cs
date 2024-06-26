﻿using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using GarageVersion3.Validation;

namespace GarageVersion3.Models.ViewModels
{
    public class UserViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Personal Identify Number")]
        [UniquePersonalNr]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "Birth Date must be 10 numbers")]
        public string PersonalIdentifyNumber { get; set; }
        
        [Display(Name = "First Name")]
        [StringLength(32, MinimumLength = 2, ErrorMessage = "First Name must be between 2 - 32")]
        [FirstNameIsNotLastName]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        [StringLength(32, MinimumLength = 2, ErrorMessage = "Last Name must be between 2 - 32")]
        public string LastName { get; set; }

        public List<Vehicle>? Vehicles { get; set; }

        [DisplayName("Number Of Vehicles")]
        public int NrOfVehicles { get; set; }

    }
}
