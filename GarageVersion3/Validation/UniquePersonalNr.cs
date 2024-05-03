using GarageVersion3.Data;
using GarageVersion3.Models.ViewModels;
using System.ComponentModel.DataAnnotations;

namespace GarageVersion3.Validation
{
    public class UniquePersonalNr : ValidationAttribute
    {
        private readonly GarageVersion3Context dbContext;
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            const string errorMessage = "Personal Number already exists.";

            if (value is string input)
            {
                if (validationContext.ObjectInstance is CreateUserViewModel viewModel)
                {
                    var dbContext = validationContext.GetRequiredService<GarageVersion3Context>();
                    
                    if (dbContext.User.Any(u => u.BirthDate == viewModel.BirthDate))
                    {
                        return new ValidationResult(errorMessage);
                    }
                    else
                    {
                        return ValidationResult.Success;
                    }
                }
            }
            return new ValidationResult("value is not a string!");
        }
    }
}
