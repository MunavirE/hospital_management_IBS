namespace WebApp.Models
{
    public class PatientAppointment
    {
        public int Id { get; set; }
        public int PatientId { get; set; }
        public int DoctorId { get; set; }
        public string Reason { get; set; }
        public DateTime AppointmentDate { get; set; }
        public string Status { get; set; }
    }
}
