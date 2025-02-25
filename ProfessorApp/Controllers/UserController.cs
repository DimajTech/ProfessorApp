using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using ProfessorApp.DTO;
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
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri($"{API_URL}/api/User/");

                    var putTask = client.PutAsJsonAsync("PutUser/" + user.Id, user);
                    putTask.Wait();

                    var result = putTask.Result;

                    if (result.IsSuccessStatusCode)
                    {
                        var updatedUser = await result.Content.ReadFromJsonAsync<User>();
                        return Json(new
                        {
                            success = true,
                            user = updatedUser
                        });
                    }
                    else
                    {
                        var errorContent = await result.Content.ReadAsStringAsync();
                        return Json(new
                        {
                            success = false,
                            message = "No se pudo actualizar el usuario",
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


        [HttpDelete]
        public IActionResult DeleteUser([FromQuery] string id)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri($"{API_URL}/api/User/");

                    var deleteTask = client.DeleteAsync("DeleteUser/" + id);
                    deleteTask.Wait();

                    var result = deleteTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        return new JsonResult(result);
                    }
                    else
                    {
                        return Json(new
                        {
                            success = false,
                            message = "No se pudo eliminar el usuario"
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }

        }
    }
}
