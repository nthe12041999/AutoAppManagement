using AutoAppManagement.Models.ViewModel;
using AutoAppManagement.WebApp.Controllers.Base;
using Microsoft.AspNetCore.Mvc;

namespace AutoAppManagement.WebApp.Controllers
{
    public class SettingsController : BaseController
    {
        public SettingsController(RestOutput res) : base(res)
        {
        }

        public IActionResult Index()
        {
            ViewData["Title"] = "Settings";
            ViewData["PageName"] = "settings";
            return View();
        }

        public IActionResult Profile()
        {
            ViewData["Title"] = "Profile Settings";
            ViewData["PageName"] = "settings-profile";
            return View();
        }

        public IActionResult Security()
        {
            ViewData["Title"] = "Security Settings";
            ViewData["PageName"] = "settings-security";
            return View();
        }

        public IActionResult Notifications()
        {
            ViewData["Title"] = "Notification Settings";
            ViewData["PageName"] = "settings-notifications";
            return View();
        }
    }
}
