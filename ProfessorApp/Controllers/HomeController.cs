using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentApp.Models;

namespace StudentApp.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }



        [Route("view/{viewName}/{id?}")]
        public IActionResult LoadView(string viewName, string? id)
        {
            return View($"~/Views/Shared/{viewName}.cshtml");
        }


    }

}
