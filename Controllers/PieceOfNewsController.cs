using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using StudentApp.Models.DAO;
using StudentApp.Models.Entity;
using System.IO.Pipelines;

namespace StudentApp.Controllers
{
    public class PieceOfNewsController : Controller
    {
        private readonly ILogger<PieceOfNewsController> _logger;
        private readonly IConfiguration _configuration;
        PieceOfNewsDAO newsDAO;

        public PieceOfNewsController(ILogger<PieceOfNewsController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;

            newsDAO = new PieceOfNewsDAO(_configuration);

        }


        [HttpGet]
        public IEnumerable<PieceOfNews> GetNews()
        {
            //TODO Manejar excepciones


            return newsDAO.Get();
        }

        [HttpPost]
        public IActionResult CreateNews([FromBody] PieceOfNews news)
        {
            try
            {
                return Ok(newsDAO.Insert(news));
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

        public IActionResult GetById(string id)
        {
            try
            {
                PieceOfNews news =  newsDAO.Get(id);

                Console.WriteLine(news);
                if(news.User is null)
                {
                    return Ok(null);

                }
                return Ok(news);

            }
            catch (Exception e)
            {
                return StatusCode(500, new { message = "An unexpected error occurred", error = e.Message });

            }
        }
    }
}
