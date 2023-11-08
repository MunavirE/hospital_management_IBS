using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Utilities;
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

        // POST: api/doctor_registration
        [HttpPost]
        public IActionResult CreateDoctor(Doctor doctor)
        {
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string insertDoctorSql = "INSERT INTO doctor_registration (email, [password], first_name, last_name, specialty) " +
                                                            "VALUES (@email, @password, @firstName, @lastName, @specialty)";
                    int doctorId;
                    using (var insertDoctorCommand = new SqlCommand(insertDoctorSql, connection))
                    {
                        insertDoctorCommand.Parameters.AddWithValue("@email", doctor.Email);
                        insertDoctorCommand.Parameters.AddWithValue("@password", doctor.Password);
                        insertDoctorCommand.Parameters.AddWithValue("@firstName", doctor.FirstName);
                        insertDoctorCommand.Parameters.AddWithValue("@lastName", doctor.LastName);
                        insertDoctorCommand.Parameters.AddWithValue("@specialty", doctor.Specialty);

                        doctorId = (int)insertDoctorCommand.ExecuteScalar();
                    }

                    // Insert doctor's schedule data if provided
                    if (doctor.Schedule != null && doctor.Schedule.Any())
                    {
                        foreach (var scheduleEntry in doctor.Schedule)
                        {
                            // Insert schedule entry for the doctor
                            string insertScheduleSql = "INSERT INTO doctor_schedule (doctor_id, day, start_time, end_time) " +
                                                       "VALUES (@doctorId, @day, @startTime, @endTime)";

                            using (var insertScheduleCommand = new SqlCommand(insertScheduleSql, connection))
                            {
                                // Assuming doctorId is obtained from the inserted doctor's ID (you may need to query it)
                                // Adjust the table and column names as per your database schema
                                insertScheduleCommand.Parameters.AddWithValue("@doctorId", doctorId);
                                insertScheduleCommand.Parameters.AddWithValue("@day", scheduleEntry.Day);
                                insertScheduleCommand.Parameters.AddWithValue("@startTime", scheduleEntry.StartTime);
                                insertScheduleCommand.Parameters.AddWithValue("@endTime", scheduleEntry.EndTime);

                                insertScheduleCommand.ExecuteNonQuery();
                            }
                        }
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

        [HttpGet]
        public IActionResult GetDoctorsWithSchedules()
        {
            List<Doctor> doctors = new List<Doctor>();

            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "SELECT * FROM doctor";

                    using (var command = new SqlCommand(sql, connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Doctor doctor = new Doctor
                                {
                                    Id = reader.GetInt32(0),
                                    Email = reader.GetString(1),
                                    Password = reader.GetString(2),
                                    FirstName = reader.GetString(3),
                                    LastName = reader.GetString(4),
                                    Specialty = reader.GetString(5),
                                };

                                doctor.Schedule = GetDoctorSchedule(connection, doctor.Id); // Retrieve schedule data

                                doctors.Add(doctor);
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

        private List<ScheduleEntry> GetDoctorSchedule(SqlConnection connection, int doctorId)
        {
            List<ScheduleEntry> schedule = new List<ScheduleEntry>();

            // Modify the query to retrieve schedule entries for the specific doctor
            string scheduleSql = "SELECT day, start_time, end_time FROM doctor_schedule WHERE doctor_id = @doctorId";

            using (var scheduleCommand = new SqlCommand(scheduleSql, connection))
            {
                scheduleCommand.Parameters.AddWithValue("@doctorId", doctorId);

                using (var scheduleReader = scheduleCommand.ExecuteReader())
                {
                    while (scheduleReader.Read())
                    {
                        ScheduleEntry scheduleEntry = new ScheduleEntry
                        {
                            Day = scheduleReader.GetString(0),
                            StartTime = scheduleReader.IsDBNull(1) ? null : scheduleReader.GetString(1),
                            EndTime = scheduleReader.IsDBNull(2) ? null : scheduleReader.GetString(2)
                        };

                        schedule.Add(scheduleEntry);
                    }
                }
            }

            return schedule;
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
