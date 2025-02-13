using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using StudentApp.Models.DAO;
using StudentApp.Models.Entity;

namespace StudentApp.Controllers
{
    public class UserController : Controller
    {
        private readonly ILogger<UserController> _logger;
        private readonly IConfiguration _configuration;
        UserDAO userDAO;
        private readonly string API_URL;

        public UserController(ILogger<UserController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;

            userDAO = new UserDAO(_configuration);
            API_URL = _configuration["EnvironmentVariables:API_URL"];

        }
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpGet]
        public IActionResult Register()
        {
            return View(); //ASP.NET Core busca automáticamente Views/Shared/Register.cshtml
        }

        [HttpPost]
        public IActionResult Login(string email, string password)
        {
            try
            {
                User user = null;

                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri($"{API_URL}/api/User/GetUserByEmail/" + email);
                    var responseTask = client.GetAsync(client.BaseAddress);
                    responseTask.Wait();

                    var result = responseTask.Result;

                    if (result.IsSuccessStatusCode)
                    {
                        var readTask = result.Content.ReadAsAsync<User>();
                        readTask.Wait();
                        //lee el estudiante provenientes de la API
                        user = readTask.Result;

                    }
                }


                bool success = false;
                string message = "";
                string userId = "";
                string role = "";
                string picture = "";

                if (user != null && user.Password == password)
                {
                    if (user.RegistrationStatus == "accepted")
                    {
                        if (user.IsActive == true)
                        {
                            if (user.Role == "professor")
                            {
                                var authData = $"{user.Email} {user.Id}";

                                var cookieOptions = new CookieOptions
                                {
                                    //HttpOnly = false,
                                    Secure = true,
                                    Expires = DateTime.UtcNow.AddHours(3)
                                };

                                Response.Cookies.Append("AuthCookieProfessor", authData, cookieOptions);

                                // Devolver los datos en la respuesta JSON
                                success = true;
                                userId = user.Id.ToString();
                                role = user.Role.ToString();
                                picture = user.Picture.ToString();
                            }
                            else
                            {
                                message = "Su usuario no es un profesor. ";

                            }

                        }
                        else
                        {
                            message = "Su usuario se encuentra inactivo. Contacte un administrador.";
                        }
                    }
                    else
                    {
                        message = "Su usuario aún no ha sido aprobado.";
                    }
                }
                else
                {
                    message = "Credenciales inválidas.";
                }

                return Json(new { success, message, userId, role, picture });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error en el servidor." });
            }
        }


		[HttpGet]
		public IActionResult GetByEmail([FromQuery] string email)
		{
			try
			{
				User user = userDAO.GetByEmail(email);

				if (user != null)
				{
					return Ok(user);
				}

				return BadRequest();
			}
			catch (SqlException e)
			{
				return StatusCode(500, new { message = "An error occurred while retrieving the user.", error = e.Message });
			}

		}

        [HttpPut]
        public IActionResult UpdateUser([FromBody] User user)
        {
            try
            {
                return Ok(userDAO.Update(user));
            }
            catch (SqlException e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpDelete]
        public IActionResult DeleteUser([FromQuery] string id)
        {
            try
            {
                var result = userDAO.Delete(id);
                return Ok(new { success = true, result = result });
            }
            catch (SqlException e)
            {
                return BadRequest(new { success = false, message = e.Message });
            }
        }
    }
}
