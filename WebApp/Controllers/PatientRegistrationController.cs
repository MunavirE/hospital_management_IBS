using Microsoft.AspNetCore.Mvc;

namespace WebApp.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using System;
    using System.Collections.Generic;
    using System.Data.SqlClient;
    using System.Linq;
    using System.Reflection;
    using System.Reflection.Metadata.Ecma335;
    using WebApp.Models;
    using static System.Net.Mime.MediaTypeNames;

    [Route("api/patient_registration")]
    [ApiController]
    public class PatientRegistrationController : ControllerBase
    {
        private readonly string connectionString;
        private object connection;
        private int i;

        public PatientRegistrationController(IConfiguration configuration)
        {
            connectionString = configuration["ConnectionStrings:SqlServerDb"] ?? "";
        }

        // POST: api/patient_registration
        [HttpPost]
        public IActionResult CreatePatient(PatientDTO patient)
        {
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "INSERT INTO patient_registration (email, [password], first_name, last_name, date_of_birth, gender, phone, address, image) " +
                         "VALUES (@email, @password, @firstName, @lastName, @dateOfBirth, @gender, @phone, @address, @image)"; ;
                    using (var command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@email", patient.Email);
                        command.Parameters.AddWithValue("@password", patient.Password);
                        command.Parameters.AddWithValue("@firstName", patient.FirstName);
                        command.Parameters.AddWithValue("@lastName", patient.LastName);
                        command.Parameters.AddWithValue("@dateOfBirth", patient.DateOfBirth);
                        command.Parameters.AddWithValue("@gender", patient.Gender);
                        command.Parameters.AddWithValue("@phone", patient.Contact.Phone);
                        command.Parameters.AddWithValue("@address", patient.Contact.Address);
                        command.Parameters.AddWithValue("@image", patient.Image);

                        command.ExecuteNonQuery();
                    }
                }
                //  return CreatedAtAction("GetPatient", new { id = patient.id }, patient);
                return Ok();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Patient Registration", "Sorry, but we have an exception" + ex);
                return BadRequest(ModelState);
            }
        }

        [HttpGet]
        public IActionResult GetPatients()
        {
            List<PatientRegistration> patients = new List<PatientRegistration>();
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "SELECT * FROM patient_registration";
                    using (var command = new SqlCommand(sql, connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                PatientRegistration patient = new PatientRegistration();
                                patient.Id = reader.GetInt32(0);
                                patient.Email = reader.GetString(1);
                                patient.FirstName = reader.GetString(3);
                                patient.LastName = reader.GetString(4);
                                patient.Gender = reader.GetString(6);
                                ContactInfo contactInfo = new ContactInfo();
                                contactInfo.Address = reader.GetString(8);
                                contactInfo.Phone = reader.GetString(7);
                                patient.Contact = contactInfo;
                                patient.DateOfBirth = reader.GetDateTime(5);
                                patient.Image = reader.GetString(9);
                                patients.Add(patient);

                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Patient Registration", "Sorry, but we have an exception" + ex);
                return BadRequest(ModelState);

            }
            return Ok(patients);
        }
        // GET: api/patient-registration/{id}
        [HttpGet("{id}")]
        public IActionResult GetPatient(int id)
        {
            PatientRegistration patient = new PatientRegistration();
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "SELECT * FROM patient_registration WHERE id=@id";
                    using (var command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("id", id);
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                patient.Id = reader.GetInt32(0);
                                patient.Email = reader.GetString(1);
                                patient.FirstName = reader.GetString(3);
                                patient.LastName = reader.GetString(4);
                                patient.Gender = reader.GetString(6);
                                ContactInfo contactInfo = new ContactInfo();
                                contactInfo.Address = reader.GetString(8);
                                contactInfo.Phone = reader.GetString(7);
                                patient.Contact = contactInfo;
                                patient.DateOfBirth = reader.GetDateTime(5);
                                patient.Image = reader.GetString(9);

                            }
                            else {
                                return NotFound();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Patient Registration", "Sorry, but we have an exception" + ex);
                return BadRequest(ModelState);

            }
            return Ok(patient);
        }

        [HttpPut("{id}")]
        public IActionResult UpdatePatient(int id, PatientRegistration patient)
        {
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "UPDATE patient_registration " +
                         "SET email = @email, [password] = @password, first_name = @firstName, last_name = @lastName, " +
                         "date_of_birth = @dateOfBirth, gender = @gender, " +
                         "phone = @phone, address = @address, image = @image " +
                         "WHERE id = @id";
                    using (var command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@id", id);
                        command.Parameters.AddWithValue("@email", patient.Email);
                        command.Parameters.AddWithValue("@password", patient.Password);
                        command.Parameters.AddWithValue("@firstName", patient.FirstName);
                        command.Parameters.AddWithValue("@lastName", patient.LastName);
                        command.Parameters.AddWithValue("@dateOfBirth", patient.DateOfBirth);
                        command.Parameters.AddWithValue("@gender", patient.Gender);
                        command.Parameters.AddWithValue("@phone", patient.Contact.Phone);
                        command.Parameters.AddWithValue("@address", patient.Contact.Address);
                        command.Parameters.AddWithValue("@image", patient.Image);
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Patient Registration", "Sorry, but we have an exception" + ex);
                return BadRequest(ModelState);
            }
            return Ok();
        }

        [HttpDelete("{id}")]
        public IActionResult DeletePatient(int id)
        {
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "DELETE FROM patient_registration WHERE id=@id";
                    using (var command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@id", id);
                        command.ExecuteNonQuery();
                    }
                  }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Patient Registration", "Sorry, but we have an exception" + ex);
                return BadRequest(ModelState);

            }
            return Ok();
        }
    }
}
