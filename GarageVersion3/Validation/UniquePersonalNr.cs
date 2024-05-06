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
            if (value is string input)
            {
                if (validationContext.ObjectInstance is CreateUserViewModel viewModel)
                {
                    try
                    {
                        Personnummer.Personnummer personNr = new Personnummer.Personnummer(value.ToString());
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        return new ValidationResult("Could not validate Personal Number.");
                    }

                    var dbContext = validationContext.GetRequiredService<GarageVersion3Context>();
                    
                    if (dbContext.User.Any(u => u.PersonalIdentifyNumber == viewModel.PersonalIdentifyNumber))
                    {
                        return new ValidationResult("Personal Number already exists.");
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
