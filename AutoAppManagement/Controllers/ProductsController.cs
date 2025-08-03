using AutoAppManagement.Models.ViewModel;
using AutoAppManagement.WebApp.Controllers.Base;
using Microsoft.AspNetCore.Mvc;

namespace AutoAppManagement.WebApp.Controllers
{
    public class ProductsController : BaseController
    {
        public ProductsController(RestOutput res) : base(res)
        {
        }

        public IActionResult Index()
        {
            ViewData["Title"] = "Products";
            ViewData["PageName"] = "products";
            return View();
        }
    }
}
