using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using StudentApp.Models.DAO;
using StudentApp.Models.Entity;
using System.Text;
using System.Text.Json;

namespace StudentApp.Controllers
{
    public class UserController : Controller
    {
        private readonly ILogger<UserController> _logger;
        private readonly IConfiguration _configuration;
        UserDAO userDAO;
        readonly string API_URL;

        public UserController(ILogger<UserController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
            API_URL = _configuration["EnvironmentVariables:API_URL"];

            userDAO = new UserDAO(_configuration);

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
                User user = userDAO.GetByEmail(email);
                bool success = false;
                string message = "";
                string userId = "";
                string role = "";
                string picture = "";

                if (user != null && user.Password == password)
                {
                    if (user.RegistrationStatus == "accepted")
                    {
                        if (user.IsActive)
                        {
                            var authData = $"{user.Email} {user.Id}";

                            var cookieOptions = new CookieOptions
                            {
                                //HttpOnly = false,
                                Secure = true,
                                Expires = DateTime.UtcNow.AddHours(3)
                            };

                            Response.Cookies.Append("AuthCookie", authData, cookieOptions);

                            // Devolver los datos en la respuesta JSON
                            success = true;
                            userId = user.Id.ToString();
                            role = user.Role.ToString();
                            picture = user.Picture.ToString();
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




        [HttpPost]
        public IActionResult Register([FromBody] User user)
        {
            bool success = false;
            string message = "";
            try
            {
                if (userDAO.GetByEmail(user.Email).Email == null)
                {
                    int result = userDAO.Insert(user);

                    if(result == 1)
                    {
                        success = true;
                        message = "Su solicitud ha sido enviada correctamente. Revisa tu correo. ";
                    }
                    else
                    {
                        success = false;
                        message = "No ha sido posible hacer la solicitud de registro. Intente de nuevo más tarde. ";
                    }
                }
                else
                {
                    success = false;
                    message = "El correo electrónico ya ha sido registrado en el sistema. ";
                }

            }
            catch (SqlException e)
            {
                 success = false;
                 message = "Ha ocurrido un error inesperado en el sistema. Intente de nuevo más tarde.";
            }
            return Json(new { success = success, message = message });

        }

		[HttpGet]
		public async Task<ActionResult<User>> GetByEmail([FromQuery] string email)
		{
            try
            {
                using (var client = new HttpClient())
                {
                    var response = await client.GetAsync($"{API_URL}/api/User/GetUserByEmail/{email}");

                    if (response.IsSuccessStatusCode)
                    {
                        var user = await response.Content.ReadFromJsonAsync<User>();
                        return Ok(user);
                    }
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }

        }

        [HttpPut]
        public async Task<IActionResult> UpdateUser([FromBody] User user)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri($"{API_URL}/api/User/");
                var putTask = client.PutAsJsonAsync("PutUser/" + user.Id, user);
                putTask.Wait();

                var result = putTask.Result;

                if (result.IsSuccessStatusCode)
                {
                    return new JsonResult(result);
                    // TODO: return new JsonResult(student);
                }
                else
                {
                    // TODO should be customized to meet the client's needs
                    return new JsonResult(result);
                }
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
