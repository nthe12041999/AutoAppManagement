using AutoAppManagement.Models.ViewModel;
using AutoAppManagement.WebApp.Controllers.Base;
using Microsoft.AspNetCore.Mvc;

namespace AutoAppManagement.WebApp.Controllers
{
    public class ElementsController : BaseController
    {
        public ElementsController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public IActionResult Index()
        {
            ViewData["Title"] = "Elements";
            ViewData["PageName"] = "elements";
            return View();
        }

        public IActionResult Buttons()
        {
            ViewData["Title"] = "Buttons";
            ViewData["PageName"] = "elements-buttons";
            return View();
        }

        public IActionResult Alerts()
        {
            ViewData["Title"] = "Alerts";
            ViewData["PageName"] = "elements-alerts";
            return View();
        }

        public IActionResult Badges()
        {
            ViewData["Title"] = "Badges";
            ViewData["PageName"] = "elements-badges";
            return View();
        }

        public IActionResult Cards()
        {
            ViewData["Title"] = "Cards";
            ViewData["PageName"] = "elements-cards";
            return View();
        }

        public IActionResult Modals()
        {
            ViewData["Title"] = "Modals";
            ViewData["PageName"] = "elements-modals";
            return View();
        }

        public IActionResult Forms()
        {
            ViewData["Title"] = "Forms";
            ViewData["PageName"] = "elements-forms";
            return View();
        }

        public IActionResult Tables()
        {
            ViewData["Title"] = "Tables";
            ViewData["PageName"] = "elements-tables";
            return View();
        }
    }
}
