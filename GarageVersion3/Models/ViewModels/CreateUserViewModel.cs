﻿using System.ComponentModel.DataAnnotations;
using GarageVersion3.Validation;

namespace GarageVersion3.Models.ViewModels
{
    public class CreateUserViewModel
    {
        public int Id { get; set; }
        [UniquePersonalNr]
        [StringLength(12, MinimumLength = 10, ErrorMessage = "Birth Date must be between 10 - 12")]
        public string BirthDate { get; set; }

        [StringLength(32, MinimumLength = 2, ErrorMessage = "First Name must be between 2 - 32")]
        [FirstNameIsNotLastName]
        public string FirstName { get; set; }

        [StringLength(32, MinimumLength = 2, ErrorMessage = "Last Name must be between 2 - 32")]
        public string LastName { get; set; }
    }
}
