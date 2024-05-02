using System.ComponentModel.DataAnnotations;

namespace GarageVersion3.Models
{
    public class User
    {
        public int Id { get; set; }

        [Range(10,12,ErrorMessage = "Birth Date must be between 10 - 12")]
        public string BirthDate { get; set; }

        [Range(2,32, ErrorMessage = "First Name must be between 2 - 32")]
        public string FirstName { get; set; }

        [Range(2, 32, ErrorMessage = "Last Name must be between 2 - 32")]
        public string LastName { get; set; }
        
        List<Receipt> Receipts { get; set; }
    }
}
