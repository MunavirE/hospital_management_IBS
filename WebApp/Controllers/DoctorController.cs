using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using WebApp.Models;

namespace WebApp.Controllers
{
    [Route("api/doctor")]
    [ApiController]
    public class DoctorController : Controller
    {
        private readonly string connectionString;

        public DoctorController(IConfiguration configuration)
        {
            connectionString = configuration["ConnectionStrings:SqlServerDb"] ?? "";
        }

        // POST: api/doctor
        [HttpPost]
        public IActionResult CreateDoctor(Doctor doctor)
        {
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string insertDoctorSql = "INSERT INTO doctor (email, [password], first_name, last_name, specialty) " +
                                                            "VALUES (@email, @password, @firstName, @lastName, @specialty)";
                    
                    using (var insertDoctorCommand = new SqlCommand(insertDoctorSql, connection))
                    {
                        insertDoctorCommand.Parameters.AddWithValue("@email", doctor.Email);
                        insertDoctorCommand.Parameters.AddWithValue("@password", doctor.Password);
                        insertDoctorCommand.Parameters.AddWithValue("@firstName", doctor.FirstName);
                        insertDoctorCommand.Parameters.AddWithValue("@lastName", doctor.LastName);
                        insertDoctorCommand.Parameters.AddWithValue("@specialty", doctor.Specialty);
                        // Ensure that insertDoctorCommand is not null and the connection is open
                        insertDoctorCommand.ExecuteNonQuery();
                    }

                }

                return Ok("Doctor registration successful");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Doctor Registration", "Sorry, but we have an exception" + ex);
                return BadRequest(ModelState);
            }
        }

        [HttpPost("schedule")]
        public IActionResult ScheduleDoctor([FromBody] ScheduleEntry scheduleEntry)
        {
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Construct the INSERT SQL query for scheduling a doctor
                    string sql = "INSERT INTO doctor_schedule (doctor_id, day, start_time, end_time) " +
                                 "VALUES (@doctorId, @day, @startTime, @endTime)";

                    using (var command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@doctorId",scheduleEntry.DoctorId);
                        command.Parameters.AddWithValue("@day", scheduleEntry.Day);
                        command.Parameters.AddWithValue("@startTime", scheduleEntry.StartTime);
                        command.Parameters.AddWithValue("@endTime", scheduleEntry.EndTime);

                        command.ExecuteNonQuery();
                    }
                }

                return Ok("Doctor scheduled successfully");
            }
            catch (Exception ex)
            {
                // Handle any exceptions and return an error response
                ModelState.AddModelError("Doctor Scheduling", "Sorry, but we have an exception: " + ex.Message);
                return BadRequest(ModelState);
            }
        }


        [HttpGet]
        public IActionResult GetDoctorsWithSchedules()
        {
            List<Doctor> doctors = new List<Doctor>();

            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "SELECT D.id, D.email, D.[password], D.first_name, D.last_name, D.specialty, S.day, S.start_time, S.end_time " +
                                 "FROM doctor D " +
                                 "LEFT JOIN doctor_schedule S ON D.id = S.doctor_id";

                    using (var command = new SqlCommand(sql, connection))
                    using (var reader = command.ExecuteReader())
                    {
                        int currentDoctorId = 0;
                        Doctor doctor = null;

                        while (reader.Read())
                        {
                            int doctorId = reader.GetInt32(0);

                            if (doctor == null || doctorId != currentDoctorId)
                            {
                                // Create a new Doctor object
                                doctor = new Doctor
                                {
                                    Id = doctorId,
                                    Email = reader.GetString(1),
                                    Password = reader.GetString(2),
                                    FirstName = reader.GetString(3),
                                    LastName = reader.GetString(4),
                                    Specialty = reader.GetString(5),
                                    Schedule = new List<ScheduleEntry>()
                                };

                                doctors.Add(doctor);
                                currentDoctorId = doctorId;
                            }

                            // Add schedule entry to the current Doctor's Schedule
                            if (!reader.IsDBNull(6))
                            {
                                ScheduleEntry scheduleEntry = new ScheduleEntry
                                {
                                    DoctorId=currentDoctorId,
                                    Day = reader.GetString(6),
                                    StartTime = reader.IsDBNull(7) ? null : reader.GetString(7),
                                    EndTime = reader.IsDBNull(8) ? null : reader.GetString(8)
                                };

                                doctor.Schedule.Add(scheduleEntry);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Doctor Registration", "Sorry, but we have an exception" + ex);
                return BadRequest(ModelState);
            }

            return Ok(doctors);
        }


        // PUT: api/doctor_registration/{id}
        [HttpPut("{id}")]
        public IActionResult UpdateDoctor(int id, Doctor updatedDoctor)
        {
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Update doctor information
                    string updateDoctorSql = "UPDATE doctor " +
                                             "SET email = @email, [password] = @password, first_name = @firstName, " +
                                             "last_name = @lastName, specialty = @specialty " +
                                             "WHERE id = @id";

                    using (var updateDoctorCommand = new SqlCommand(updateDoctorSql, connection))
                    {
                        updateDoctorCommand.Parameters.AddWithValue("@id", id);
                        updateDoctorCommand.Parameters.AddWithValue("@email", updatedDoctor.Email);
                        updateDoctorCommand.Parameters.AddWithValue("@password", updatedDoctor.Password);
                        updateDoctorCommand.Parameters.AddWithValue("@firstName", updatedDoctor.FirstName);
                        updateDoctorCommand.Parameters.AddWithValue("@lastName", updatedDoctor.LastName);
                        updateDoctorCommand.Parameters.AddWithValue("@specialty", updatedDoctor.Specialty);

                        updateDoctorCommand.ExecuteNonQuery();
                    }

                    // Update doctor's schedule if provided
                    if (updatedDoctor.Schedule != null && updatedDoctor.Schedule.Any())
                    {
                        // Assuming that the schedule data is provided in the same format as when creating a doctor
                        // You can loop through and update the schedule entries
                        foreach (var scheduleEntry in updatedDoctor.Schedule)
                        {
                            // Update schedule entry for the doctor
                            string updateScheduleSql = "UPDATE doctor_schedule " +
                                                       "SET start_time = @startTime, end_time = @endTime " +
                                                       "WHERE doctor_id = @doctorId AND day = @day";

                            using (var updateScheduleCommand = new SqlCommand(updateScheduleSql, connection))
                            {
                                updateScheduleCommand.Parameters.AddWithValue("@doctorId", id);
                                updateScheduleCommand.Parameters.AddWithValue("@day", scheduleEntry.Day);
                                updateScheduleCommand.Parameters.AddWithValue("@startTime", scheduleEntry.StartTime);
                                updateScheduleCommand.Parameters.AddWithValue("@endTime", scheduleEntry.EndTime);

                                updateScheduleCommand.ExecuteNonQuery();
                            }
                        }
                    }
                }

                return Ok("Doctor information and schedule updated successfully");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Doctor Update", "Sorry, but we have an exception" + ex);
                return BadRequest(ModelState);
            }
        }

        // DELETE: api/doctor_registration/{id}
        [HttpDelete("{id}")]
        public IActionResult DeleteDoctor(int id)
        {
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Delete doctor information
                    string deleteDoctorSql = "DELETE FROM doctor WHERE id = @id";

                    using (var deleteDoctorCommand = new SqlCommand(deleteDoctorSql, connection))
                    {
                        deleteDoctorCommand.Parameters.AddWithValue("@id", id);
                        int rowsAffected = deleteDoctorCommand.ExecuteNonQuery();

                        if (rowsAffected == 0)
                        {
                            return NotFound();
                        }
                    }

                    // Delete doctor's schedule
                    string deleteScheduleSql = "DELETE FROM doctor_schedule WHERE doctor_id = @doctorId";

                    using (var deleteScheduleCommand = new SqlCommand(deleteScheduleSql, connection))
                    {
                        deleteScheduleCommand.Parameters.AddWithValue("@doctorId", id);
                        deleteScheduleCommand.ExecuteNonQuery();
                    }
                }

                return Ok("Doctor and schedule deleted successfully");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Doctor Deletion", "Sorry, but we have an exception" + ex);
                return BadRequest(ModelState);
            }
        }

    }
}
