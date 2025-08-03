using AutoAppManagement.Models.ViewModel;
using AutoAppManagement.WebApp.Controllers.Base;
using Microsoft.AspNetCore.Mvc;

namespace AutoAppManagement.WebApp.Controllers
{
    public class OrdersController : BaseController
    {
        public OrdersController(RestOutput res) : base(res)
        {
        }

        public IActionResult Index()
        {
            ViewData["Title"] = "Orders";
            ViewData["PageName"] = "orders";
            return View();
        }
    }
}
