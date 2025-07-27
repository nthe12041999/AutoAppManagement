using AutoAppManagement.API.Common.Attribute;
using AutoAppManagement.API.Controllers.Base;
using AutoAppManagement.Models.Constant;
using AutoAppManagement.Models.ViewModel;
using AutoAppManagement.WebApp.Services;
using Microsoft.AspNetCore.Mvc;

namespace AutoAppManagement.API.Controllers
{
    public class NotificationController : BaseController
    {
        private readonly INotificationService _notificationService;

        public NotificationController(IRestOutput res, IHttpContextAccessor httpContextAccessor,
                                    INotificationService notificationService) : base(res, httpContextAccessor)
        {
            _notificationService = notificationService;

        }

        /// <summary>
        /// </summary>
        /// <param name="acc"></param>
        /// <returns></returns>
        [HttpGet("GetCountNotificationUnReadByAcc")]
        [Roles(RoleConstant.Customer)]
        public async Task<IActionResult> GetCountNotificationUnReadByAcc()
        {
            var countNotification = await _notificationService.GetCountNotificationUnReadByAcc();
            _res.SuccessEventHandler(countNotification);
            return Ok(_res);
        }

        [HttpPost("MaskAsRead")]
        [Roles(RoleConstant.Customer)]
        public async Task<IActionResult> MaskAsRead(long noticeId)
        {
            var countNotification = await _notificationService.MaskAsRead(noticeId);
            _res.SuccessEventHandler(countNotification);
            return Ok(_res);
        }

        [HttpGet("GetNoticeByRange")]
        [Roles(RoleConstant.Customer)]
        public async Task<IActionResult> GetNoticeByRange(int from, int to)
        {
            var countNotification = await _notificationService.GetNoticeByRange(from, to);
            _res.SuccessEventHandler(countNotification);
            return Ok(_res);
        }
    }
}
