using AutoAppManagement.Models.ViewModel;
using AutoAppManagement.WebApp.Controllers.Base;
using Microsoft.AspNetCore.Mvc;

namespace AutoAppManagement.WebApp.Controllers
{
    public class CalendarController : BaseController
    {
        public CalendarController(RestOutput res) : base(res)
        {
        }

        public IActionResult Index()
        {
            ViewData["Title"] = "Calendar";
            ViewData["PageName"] = "calendar";
            return View();
        }
    }
}
