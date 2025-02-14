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
        readonly string API_URL;
        public AdvisementController(ILogger<AdvisementController> logger, IConfiguration configuration)
        {

            _logger = logger;
            _configuration = configuration;
            API_URL = _configuration["EnvironmentVariables:API_URL"];
            advisementDAO = new AdvisementDAO(_configuration);
        }

        public IActionResult Index()
        {
            return View();
        }
        /*
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
        */

        [HttpGet]
        public async Task<IActionResult> GetAdvisementById([FromQuery] string id)
        {
            try
            {
                using (var client = new HttpClient())
                {
                  
                    var response = await client.GetAsync($"{API_URL}/api/Advisement/GetAdvisementById/{id}");

                    if (response.IsSuccessStatusCode)
                    {
                        var advisement = await response.Content.ReadFromJsonAsync<Advisement>();
                        return Ok(advisement);
                    }
                    else
                    {
                        return StatusCode((int)response.StatusCode, new { message = "Error fetching advisement", error = response.ReasonPhrase });
                    }
                }
            }
            catch (HttpRequestException e)
            {
                return StatusCode(500, new { message = "Request error", error = e.Message });
            }
            catch (Exception e)
            {
                return StatusCode(500, new { message = "Unexpected error", error = e.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAdvisementsByUser([FromQuery] string email)
        {
            try
            {
                using (var client = new HttpClient())
                {
                   
                    var response = await client.GetAsync($"{API_URL}/api/Advisement/GetMyAdvisements/{email}");

                    if (response.IsSuccessStatusCode)
                    {
                        var advisements = await response.Content.ReadFromJsonAsync<List<Advisement>>();
                        return Ok(advisements);
                    }
                    else
                    {
                        return StatusCode((int)response.StatusCode, new { message = "Error fetching advisements", error = response.ReasonPhrase });
                    }
                }
            }
            catch (HttpRequestException e)
            {
                return StatusCode(500, new { message = "Request error", error = e.Message });
            }
            catch (Exception e)
            {
                return StatusCode(500, new { message = "Unexpected error", error = e.Message });
            }
        }




        [HttpGet]
        public async Task<IActionResult> GetPublicAdvisements([FromQuery] string email)
        {
            try
            {
                using (var client = new HttpClient())
                {
               
                    var response = await client.GetAsync($"{API_URL}/api/Advisement/GetPublicAdvisements/{email}");

                    if (response.IsSuccessStatusCode)
                    {
                        var advisements = await response.Content.ReadFromJsonAsync<List<Advisement>>();
                        return Ok(advisements);
                    }
                    else
                    {
                        return StatusCode((int)response.StatusCode, new { message = "Error fetching public advisements", error = response.ReasonPhrase });
                    }
                }
            }
            catch (HttpRequestException e)
            {
                return StatusCode(500, new { message = "Request error", error = e.Message });
            }
            catch (Exception e)
            {
                return StatusCode(500, new { message = "Unexpected error", error = e.Message });
            }
        }


        public IActionResult GetAdvisementResponsesById(string id)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(API_URL);

                    var getTask = client.GetAsync($"api/Advisement/GetResponsesByAdvisement/{id}");

                    getTask.Wait();

                    var result = getTask.Result;


                    if (result.IsSuccessStatusCode)
                    {
                        var readTask = result.Content.ReadAsAsync<List<ResponseAdvisement>>();
                        readTask.Wait();

                        return Ok(readTask.Result);
                    }
                    else
                    {
                        return StatusCode((int)result.StatusCode, new { message = "Error getting responses" });
                    }
                }
            }
            catch (HttpRequestException e)
            {
                return StatusCode(500, new { message = "Server error", error = e.Message });
            }
        }
        [HttpPost]
        public IActionResult AddNewResponse([FromBody] ResponseAdvisement response)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(API_URL);


                    var postTask = client.PostAsJsonAsync("/api/Advisement/CreateResponseAdvisement", new
                    {
                        AdvisementId = response.AdvisementId,
                        UserId = response.User.Id,
                        Text = response.Text
                    });

                    postTask.Wait();

                    var result = postTask.Result;

                    if (result.IsSuccessStatusCode)
                    {
                        var readTask = result.Content.ReadAsAsync<ResponseAdvisement>();
                        readTask.Wait();
                        return Ok(readTask.Result);
                    }
                    else
                    {
                        return StatusCode((int)result.StatusCode, new { message = "Failed to add response" });
                    }
                }
            }
            catch (Exception e)
            {
                return StatusCode(500, new { message = "An error occurred", error = e.Message });
            }
        }

    }
}
