using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using WebApp.Models;

namespace WebApp.Controllers
{

    [Route("api/appointment")]
    [ApiController]
    public class AppointmentController : Controller
    {
        private readonly string connectionString;

        public AppointmentController(IConfiguration configuration)
        {
            connectionString = configuration["ConnectionStrings:SqlServerDb"] ?? "";
        }

        [HttpPost]
        public IActionResult CreateAppointment(PatientAppointment appointment)
        {
            try
            {
                // Set the default status to "Scheduled" if not provided
                if (string.IsNullOrEmpty(appointment.Status))
                {
                    appointment.Status = "Scheduled";
                }
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string insertAppointmentSql = "INSERT INTO appointment (patientId, doctorId, reason, appointmentDate, status) " +
                                      "VALUES (@patientId, @doctorId, @reason, @appointmentDate, @status)";

                    using (var insertAppointmentCommand = new SqlCommand(insertAppointmentSql, connection))
                    {
                        insertAppointmentCommand.Parameters.AddWithValue("@patientId", appointment.PatientId);
                        insertAppointmentCommand.Parameters.AddWithValue("@doctorId", appointment.DoctorId);
                        insertAppointmentCommand.Parameters.AddWithValue("@reason", appointment.Reason);
                        insertAppointmentCommand.Parameters.AddWithValue("@appointmentDate", appointment.AppointmentDate);
                        insertAppointmentCommand.Parameters.AddWithValue("@status", appointment.Status);

                        insertAppointmentCommand.ExecuteNonQuery();
                    }
                }
                // Return a success response
                return Ok(appointment);
            }
            catch (Exception ex)
            {
                // Handle any exceptions and return an error response
                ModelState.AddModelError("Appointment Creation", "Sorry, but we have an exception: " + ex.Message);
                return BadRequest(ModelState);
            }
        }

        [HttpGet]
        public IActionResult GetDoctorsWithSchedules()
        {
            try
            {
                List<PatientAppointment> appointments = new List<PatientAppointment>();

                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "SELECT * FROM appointment";

                    using (var command = new SqlCommand(sql, connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {

                                PatientAppointment appointment = new PatientAppointment
                                {
                                    Id = reader.GetInt32(0),
                                    PatientId = reader.GetInt32(1),
                                    DoctorId = reader.GetInt32(2),
                                    Reason = reader.IsDBNull(3) ? null : reader.GetString(3),
                                    AppointmentDate = reader.GetDateTime(4),
                                    Status = reader.GetString(5)
                                };

                                appointments.Add(appointment);
                            }

                            // Now, 'appointments' contains the list of appointments retrieved from the database.
                        }
                    }
                }
                return Ok(appointments);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Doctor Registration", "Sorry, but we have an exception" + ex);
                return BadRequest(ModelState);
            }
        }

        [HttpGet("{id}")]
        public IActionResult GetAppointment(int id)
        {
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "SELECT * FROM appointment WHERE id = @id";

                    using (var command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@id", id);

                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                PatientAppointment appointment = new PatientAppointment()
                                {
                                    Id=reader.GetInt32(0),
                                    PatientId = reader.GetInt32(1),
                                    DoctorId = reader.GetInt32(2),
                                    Reason = reader.IsDBNull(3) ? null : reader.GetString(3),
                                    AppointmentDate = reader.GetDateTime(4),
                                    Status = reader.GetString(5)
                                };
                                return Ok(appointment);
                                // Now, 'appointment' contains the specific appointment retrieved from the database.
                            }
                            else
                            {
                                return NotFound();
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                // Handle any exceptions and return an error response
                ModelState.AddModelError("Appointment Retrieval", "Sorry, but we have an exception: " + ex);
                return BadRequest(ModelState);
            }
        }

        [HttpPut("{id}")]
        public IActionResult UpdateAppointment(int id, [FromBody] PatientAppointment updatedAppointment)
        {
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Construct the UPDATE SQL query
                    string sql = "UPDATE appointment " +
                                 "SET patientId = @patientId, doctorId = @doctorId, reason = @reason, " +
                                 "appointmentDate = @appointmentDate, status = @status " +
                                 "WHERE id = @id";

                    using (var command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@id", id);
                        command.Parameters.AddWithValue("@patientId", updatedAppointment.PatientId);
                        command.Parameters.AddWithValue("@doctorId", updatedAppointment.DoctorId);
                        command.Parameters.AddWithValue("@reason", updatedAppointment.Reason);
                        command.Parameters.AddWithValue("@appointmentDate", updatedAppointment.AppointmentDate);
                        command.Parameters.AddWithValue("@status", updatedAppointment.Status);

                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            return Ok(updatedAppointment); // Successful update
                        }
                        else
                        {
                            return NotFound("Appointment not found"); // No rows were updated, appointment not found
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle any exceptions and return an error response
                ModelState.AddModelError("Appointment Update", "Sorry, but we have an exception: " + ex.Message);
                return BadRequest(ModelState);
            }


        }

        [HttpDelete("{id}")]
        public IActionResult DeleteAppointment(int id)
        {
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Construct the DELETE SQL query
                    string sql = "DELETE FROM appointment WHERE id = @id";

                    using (var command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@id", id);

                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            return Ok("Appointment deleted successfully");
                        }
                        else
                        {
                            return NotFound("Appointment not found"); // No rows were deleted, appointment not found
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle any exceptions and return an error response
                ModelState.AddModelError("Appointment Deletion", "Sorry, but we have an exception: " + ex.Message);
                return BadRequest(ModelState);
            }
        }

    }
}
