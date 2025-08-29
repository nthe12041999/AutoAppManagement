using AutoAppManagement.Models.ViewModel;
using AutoAppManagement.WebApp.Controllers.Base;
using Microsoft.AspNetCore.Mvc;

namespace AutoAppManagement.WebApp.Controllers
{
    public class ReportsController : BaseController
    {
        public ReportsController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public IActionResult Index()
        {
            ViewData["Title"] = "Reports";
            ViewData["PageName"] = "reports";
            return View();
        }
    }
}
