using GarageVersion3.Models.ViewModels;
using System.ComponentModel.DataAnnotations;

namespace GarageVersion3.Validation
{
    public class FirstNameIsNotLastName : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            const string errorMessage = "First and last name can not be the same.";

            if (value is string input)
            {
                if(validationContext.ObjectInstance is UserViewModel viewModel)
                {
                    if(viewModel.LastName.Replace(" ","").ToUpper().Trim() != input.Replace(" ","").ToUpper().Trim())
                    {
                        return ValidationResult.Success;
                    } else
                    {
                        return new ValidationResult(errorMessage);
                    }
                }
            }
            return new ValidationResult("input value is not string!");
        }
    }
}
