using System.ComponentModel.DataAnnotations;

namespace OceanicAirlines.Models
{
    public class User
    {
        public int UserID { get; set; }

        [Required]
        public string UserName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string PasswordHash { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
