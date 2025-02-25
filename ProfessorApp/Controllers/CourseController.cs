using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using StudentApp.Models.DAO;

namespace StudentApp.Controllers
{
    public class CourseController : Controller
    {


        private readonly ILogger<CourseController> _logger;
        private readonly IConfiguration _configuration;

        CourseDAO courseDAO;

        public CourseController(ILogger<CourseController> logger, IConfiguration configuration)
        {

            _logger = logger;
            _configuration = configuration;
            courseDAO = new CourseDAO(_configuration);
        }
        public IActionResult Index()
        {
            return View();
        }



        public IActionResult GetAllCourses()
        {
           
            return Ok(courseDAO.Get());

        }

        public IActionResult GetCourseByCode(string code)
        {
          
            return Ok(courseDAO.GetByCode(code));

        }



    }
}
