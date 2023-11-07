namespace WebApp.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Numerics;

    public class PatientRegistration
    {
        public int Id { get; set; }
        public string Email { get; set; } = "";

        public string Password { get; set; } = "";

        public string FirstName { get; set; } = "";

        public string LastName { get; set; } = "";

        public DateTime? DateOfBirth { get; set; }

        public string Gender { get; set; } = "";

        public ContactInfo? Contact { get; set; }

        public string Image { get; set; } = "";
    }

    public enum GenderEnum
    {
        Male,
        Female,
        Other
    }

    public class ContactInfo
    {
        [Required]
        [MaxLength(20)]
        public string Phone { get; set; } = "";

        [MaxLength(100)]
        public string Address { get; set; } = "";
    }

}
