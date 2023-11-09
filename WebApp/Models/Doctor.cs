using System.ComponentModel.DataAnnotations;

namespace WebApp.Models
{
    public class Doctor
    {
        public int Id { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public string Specialty { get; set; }

        public List<ScheduleEntry>? Schedule { get; set; }
    }
}
