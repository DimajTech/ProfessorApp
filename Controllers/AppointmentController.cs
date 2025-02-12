using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using StudentApp.Models.DAO;
using StudentApp.Models.Entity;

namespace StudentApp.Controllers
{
    public class AppointmentController : Controller
    {
        private readonly ILogger<AppointmentController> _logger;
        private readonly IConfiguration _configuration;

        AppointmentDAO appointmentDAO;

        public AppointmentController(ILogger<AppointmentController> logger, IConfiguration configuration)
        {
            
            _logger = logger;
            _configuration = configuration;
            appointmentDAO = new AppointmentDAO(_configuration);
        }

        public IActionResult Index()
        {
            return View();
        }



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



        public IActionResult GetAppointmentById([FromBody] Guid id)
        {
            return Ok(appointmentDAO.GetAppointment(id));


        }



        public IActionResult GetAllAppointmentsByUser([FromQuery] string email) {

            return Json(appointmentDAO.GetAll(email));

        }

    }
}
