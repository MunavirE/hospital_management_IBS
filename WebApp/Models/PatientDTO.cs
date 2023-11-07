using System.ComponentModel.DataAnnotations;

namespace WebApp.Models
{
    public class PatientDTO
    {
        [Required]
        public string Email { get; set; } = "";

        [Required]
        public string? FirstName { get; set; } = "";
        [Required]
        public string LastName { get; set; } = "";
        [Required]
        public DateTime? DateOfBirth { get; set; }
        [Required]
        [EnumDataType(typeof(GenderEnum))]
        public string Gender { get; set; } = "";
        public ContactInfoDTO ? Contact { get; set; } 
        public string Image { get; set; } = "";
        public object Password { get; internal set; } = "";
    }

    public class ContactInfoDTO
    {
        public string Phone { get; set; } = "";
        public string Address { get; set; }= "";
    }
}
