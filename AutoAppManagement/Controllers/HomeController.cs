using AutoAppManagement.WebApp.Controllers.Base;
using Microsoft.AspNetCore.Mvc;

namespace AutoAppManagement.WebApp.Controllers
{
    public class HomeController : BaseController
    {
        public HomeController(IServiceProvider serviceProvider) : base(serviceProvider)
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
