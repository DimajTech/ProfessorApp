using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using ProfessorApp.DTO;
using StudentApp.Models.DAO;
using StudentApp.Models.Entity;

namespace StudentApp.Controllers
{
    public class AppointmentController : Controller
    {
        private readonly ILogger<AppointmentController> _logger;
        private readonly IConfiguration _configuration;

        private readonly string API_URL;

        public AppointmentController(ILogger<AppointmentController> logger, IConfiguration configuration)
        {

            _logger = logger;
            _configuration = configuration;

            API_URL = _configuration["EnvironmentVariables:API_URL"];
        }

        public IActionResult Index()
        {
            return View();
        }

        public IEnumerable<Appointment> GetAppointments([FromBody] AppointmentsFilterDTO data)
        {
            IEnumerable<Appointment> appointments = null;

            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri($"{API_URL}/api/Appointment/GetAppointments");

                    var postTask = client.PostAsJsonAsync("GetAppointments", data);
                    postTask.Wait();

                    var result = postTask.Result;

                    if (result.IsSuccessStatusCode)
                    {
                        var readTask = result.Content.ReadAsAsync<List<Appointment>>();
                        readTask.Wait();
                        //lee el estudiante provenientes de la API
                        appointments = readTask.Result;
                    }

                }
            }
            catch
            {
                ModelState.AddModelError(string.Empty, "Server error. Please contact an administrator");

            }
            return appointments;
        }

        [HttpGet]
        [Route("Appointment/GetReviewedAppointments/{professorId}")]
        public IEnumerable<Appointment> GetReviewedAppointments(string professorId)
        {
            IEnumerable<Appointment> appointments = null;

            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(API_URL);

                    var getTask = client.GetAsync($"/api/Appointment/GetReviewedAppointments/{professorId}");
                    getTask.Wait();

                    var result = getTask.Result;

                    if (result.IsSuccessStatusCode)
                    {
                        var readTask = result.Content.ReadAsAsync<List<Appointment>>();
                        readTask.Wait();
                        // Lee los datos provenientes de la API
                        appointments = readTask.Result;
                    }
                }
            }
            catch
            {
                ModelState.AddModelError(string.Empty, "Server error. Please contact an administrator.");
            }

            return appointments;
        }

        [HttpGet]
        [Route("Appointment/GetAppointmentById/{appointmentID}")]
        public IActionResult GetAppointmentById(string appointmentID)
        {
            Appointment appointment = null;

            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(API_URL);

                    var getTask = client.GetAsync($"/api/Appointment/GetAppointmentById/{appointmentID}");
                    getTask.Wait();

                    var result = getTask.Result;

                    if (result.IsSuccessStatusCode)
                    {
                        var readTask = result.Content.ReadAsAsync<Appointment>();
                        readTask.Wait();
                        // Lee los datos provenientes de la API
                        appointment = readTask.Result;
                    }
                }
            }
            catch
            {
                ModelState.AddModelError(string.Empty, "Server error. Please contact an administrator.");
            }

            return Ok(appointment);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateAppointment([FromBody] Appointment appointment)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri($"{API_URL}/api/Appointment/");
                    var putTask = client.PutAsJsonAsync("PutAppointment/", new {
                        appointment.Id,
                        appointment.Status,
                        appointment.ProfessorComment
                    });

                    putTask.Wait();

                    var result = putTask.Result;

                    if (result.IsSuccessStatusCode)
                    {
                        var updatedAppointment = await result.Content.ReadFromJsonAsync<Appointment>();
                        return Json(new
                        {
                            success = true,
                            appointment = updatedAppointment
                        });
                    }
                    else
                    {
                        var errorContent = await result.Content.ReadAsStringAsync();
                        return Json(new
                        {
                            success = false,
                            message = "No se pudo actualizar la cita",
                            details = errorContent
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        /*
        public IActionResult CreateNewAppointment([FromBody] Appointment appointment)
        {
            try
            {
                return Ok(appointmentDAO.CreateAppointment(appointment));
            }
            catch (SqlException e)
            {
                return StatusCode(500, new { message = "An error occurred", error = e.Message });
            }
            catch (Exception e)
            {
                return StatusCode(500, new { message = "An unexpected error occurred", error = e.Message });
            }
        }



        public IActionResult GetAppointmentById([FromQuery] string id)
        {
            return Ok(appointmentDAO.GetAppointment(id));


        }



        public IActionResult GetAllAppointmentsByUser([FromQuery] string email) {

            return Json(appointmentDAO.GetAll(email));

        }
        */

    }
}
