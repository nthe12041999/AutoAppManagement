using AutoAppManagement.Models.ViewModel;
using AutoAppManagement.WebApp.Controllers.Base;
using Microsoft.AspNetCore.Mvc;

namespace AutoAppManagement.WebApp.Controllers
{
    public class AnalyticsController : BaseController
    {
        public AnalyticsController(RestOutput res) : base(res)
        {
        }

        public IActionResult Index()
        {
            ViewData["Title"] = "Analytics";
            ViewData["PageName"] = "analytics";
            return View();
        }
    }
}
