using System.ComponentModel.DataAnnotations;

namespace WebApp.Models
{

    public class ScheduleEntry
    {
        public int Id { get; set; }

        [Required]
        public int DoctorId { get; set; }

        [Required]
        [EnumDataType(typeof(DayOfWeek))]
        public string Day { get; set; }

        [MaxLength(8)] // Assuming a format like "HH:mm tt"
        public string StartTime { get; set; }

        [MaxLength(8)] // Assuming a format like "HH:mm tt"
        public string EndTime { get; set; }
    }
}
