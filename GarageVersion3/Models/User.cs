﻿using System.ComponentModel.DataAnnotations;

namespace GarageVersion3.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [StringLength(10, MinimumLength = 10, ErrorMessage = "Birth Date must be 10 numbers")]
        public string PersonalIdentifyNumber { get; set; }

        [StringLength(32, MinimumLength = 2, ErrorMessage = "First Name must be between 2 - 32")]
        public string FirstName { get; set; }

        [StringLength(32, MinimumLength = 2, ErrorMessage = "Last Name must be between 2 - 32")]
        public string LastName { get; set; }

        public List<Vehicle>? Vehicles { get; set; }
        public List<Receipt>? Receipts { get; set; }
    }
}
