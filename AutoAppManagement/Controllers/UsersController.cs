using AutoAppManagement.Models.ViewModel;
using AutoAppManagement.WebApp.Controllers.Base;
using Microsoft.AspNetCore.Mvc;

namespace AutoAppManagement.WebApp.Controllers
{
    public class UsersController : BaseController
    {
        public UsersController(RestOutput res) : base(res)
        {
        }

        public IActionResult Index()
        {
            ViewData["Title"] = "Users";
            ViewData["PageName"] = "users";
            return View();
        }
    }
}
