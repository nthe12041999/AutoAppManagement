using AutoAppManagement.Models.ViewModel;
using Microsoft.AspNetCore.Mvc;

namespace AutoAppManagement.WebApp.Controllers.Base
{
    public class BaseController : Controller
    {
        protected RestOutput _res;
        public BaseController(RestOutput res)
        {
            _res = res;
        }

        #region Private Method

        #endregion
    }
}
