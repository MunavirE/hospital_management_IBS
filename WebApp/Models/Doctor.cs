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

        public List<ScheduleEntry> Schedule { get; set; }
    }

    public class ScheduleEntry
    {
        [Required]
        [EnumDataType(typeof(DayOfWeek))]
        public string Day { get; set; }

        [MaxLength(8)] // Assuming a format like "HH:mm tt"
        public string StartTime { get; set; }

        [MaxLength(8)] // Assuming a format like "HH:mm tt"
        public string EndTime { get; set; }
    }
}
