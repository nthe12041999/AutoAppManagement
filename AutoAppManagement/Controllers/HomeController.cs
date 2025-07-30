using AutoAppManagement.Models.ViewModel;
using AutoAppManagement.WebApp.Controllers.Base;
using Microsoft.AspNetCore.Mvc;

namespace AutoAppManagement.WebApp.Controllers
{
    public class HomeController : BaseController
    {
        public HomeController(RestOutput res) : base(res)
        {

        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult SpikeDemo()
        {
            return View();
        }
    }
}
