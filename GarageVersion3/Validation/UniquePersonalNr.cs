using GarageVersion3.Data;
using GarageVersion3.Models.ViewModels;
using System.ComponentModel.DataAnnotations;

namespace GarageVersion3.Validation
{
    public class UniquePersonalNr : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is string input)
            {
                if (validationContext.ObjectInstance is UserViewModel viewModel)
                {                    
                    try
                    {
                        Personnummer.Personnummer personNr = new Personnummer.Personnummer(value.ToString());
                        if (personNr.Age < 18)
                        {
                            return new ValidationResult("People under 18 can not register!");
                        }
                    }

                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        return new ValidationResult("Could not validate Personal Number.");
                    }
                    
                    var dbContext = validationContext.GetRequiredService<GarageVersion3Context>();
                    
                    if (dbContext.User.Any(u => u.Id != viewModel.Id && u.PersonalIdentifyNumber == viewModel.PersonalIdentifyNumber))
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
