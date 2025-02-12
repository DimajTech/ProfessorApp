using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using StudentApp.Models.DAO;
using StudentApp.Models.Entity;

namespace StudentApp.Controllers
{
    public class AdvisementController : Controller
    {
        private readonly ILogger<AdvisementController> _logger;
        private readonly IConfiguration _configuration;

        AdvisementDAO advisementDAO;

        public AdvisementController(ILogger<AdvisementController> logger, IConfiguration configuration)
        {

            _logger = logger;
            _configuration = configuration;
            advisementDAO = new AdvisementDAO(_configuration);
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CreateNewAdvisement([FromBody] Advisement advisement)
        {
            try
            {
                return Ok(advisementDAO.Create(advisement));
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


        [HttpGet]

        public IActionResult GetAdvisementById([FromQuery] string id)
        {
            try
            {
                return Ok(advisementDAO.GetById(id));
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


        [HttpGet]
        public IActionResult GetAdvisementsByUser([FromQuery] string email)
        {
            try
            {
                return Json(advisementDAO.GetByUser(email));
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


        [HttpGet]
        public IActionResult GetPublicAdvisements([FromQuery] string email)
        {
            try
            {
                return Json(advisementDAO.GetPublicAdvisements(email));
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

        public IActionResult GetAdvisementResponsesById(string id)
        {
            try
            {
                return Ok(advisementDAO.GetAdvisementResponsesById(id));
            }
            catch (SqlException e)
            {
                return StatusCode(500, new { message = "An error occurred", error = e.Message });
            }
        }
        [HttpPost]
        public IActionResult AddNewResponse([FromBody] ResponseAdvisement response)
        {
            try
            {

                return Ok(advisementDAO.InsertNewResponse(response));
            }
            catch (SqlException e)
            {
                ViewBag.Message = e.Message;
                return StatusCode(500, new { message = "An error ocurred", error = e.Message });
            }
        }

    }
}
