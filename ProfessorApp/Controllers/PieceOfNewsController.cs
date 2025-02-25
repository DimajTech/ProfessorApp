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
        private readonly string API_URL;

        public PieceOfNewsController(ILogger<PieceOfNewsController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
            API_URL = _configuration["EnvironmentVariables:API_URL"];

        }

        //Primero
        [HttpGet]
        public IEnumerable<PieceOfNews> GetNews()
        {
            IEnumerable<PieceOfNews> pieceOfNews = null;

            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri($"{API_URL}/api/PieceOfNews/GetPieceOfNews");
                    var responseTask = client.GetAsync("GetPieceOfNews");
                    responseTask.Wait();

                    var result = responseTask.Result;

                    if (result.IsSuccessStatusCode)
                    {
                        var readTask = result.Content.ReadAsAsync<IList<PieceOfNews>>();
                        readTask.Wait();

                        pieceOfNews = readTask.Result;
                    }
                    else
                    {
                        pieceOfNews = Enumerable.Empty<PieceOfNews>();
                    }
                }
            }
            catch
            {

                ModelState.AddModelError(string.Empty, "Server error. Please contact an administrator");

            }

            return pieceOfNews;
        }

        public IActionResult GetById(string id)
        {
            PieceOfNews news = null;

            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri($"{API_URL}/api/PieceOfNews/GetPieceOfNews/" + id);
                    var responseTask = client.GetAsync(client.BaseAddress);
                    responseTask.Wait();

                    var result = responseTask.Result;

                    if (result.IsSuccessStatusCode)
                    {
                        var readTask = result.Content.ReadAsAsync<PieceOfNews>();
                        readTask.Wait();
                        //lee el estudiante provenientes de la API
                        news = readTask.Result;

                    }
                }
            }
            catch
            {
                ModelState.AddModelError(string.Empty, "Server error. Please contact an administrator");

            }


            return Ok(news);
        }
    }
}
