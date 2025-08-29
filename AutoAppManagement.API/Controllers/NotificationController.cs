using AutoAppManagement.API.Common.Attribute;
using AutoAppManagement.API.Controllers.Base;
using AutoAppManagement.Models.BaseEntity;
using AutoAppManagement.Models.Constant;
using AutoAppManagement.Models.DTO.Notification;
using AutoAppManagement.Service.Services;
using Microsoft.AspNetCore.Mvc;

namespace AutoAppManagement.API.Controllers
{
    public class NotificationController : BaseBusinessController<INotificationService, Notification, NotificationDTO>
    {
        public NotificationController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        /// <summary>
        /// </summary>
        /// <param name="acc"></param>
        /// <summary>
        /// Lấy số lượng thông báo chưa đọc
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetCountNotificationUnReadByAcc")]
        [Roles(RoleConstant.Customer)]
        public IActionResult GetCountNotificationUnReadByAcc()
        {
            try
            {
                // TODO: Method này cần implement trong INotificationService
                ResOutput.ErrorEventHandler("Method chưa được implement");
                return BadRequest(ResOutput);
            }
            catch (Exception ex)
            {
                ResOutput.ErrorEventHandler(ex.Message);
                return BadRequest(ResOutput);
            }
        }

        /// <summary>
        /// Đánh dấu đã đọc
        /// </summary>
        /// <param name="noticeId"></param>
        /// <returns></returns>
        [HttpPost("MaskAsRead")]
        [Roles(RoleConstant.Customer)]
        public IActionResult MaskAsRead(long noticeId)
        {
            try
            {
                // TODO: Method này cần implement trong INotificationService
                ResOutput.ErrorEventHandler("Method chưa được implement");
                return BadRequest(ResOutput);
            }
            catch (Exception ex)
            {
                ResOutput.ErrorEventHandler(ex.Message);
                return BadRequest(ResOutput);
            }
        }

        /// <summary>
        /// Lấy thông báo theo khoảng
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        [HttpGet("GetNoticeByRange")]
        [Roles(RoleConstant.Customer)]
        public IActionResult GetNoticeByRange(int from, int to)
        {
            try
            {
                // TODO: Method này cần implement trong INotificationService
                ResOutput.ErrorEventHandler("Method chưa được implement");
                return BadRequest(ResOutput);
            }
            catch (Exception ex)
            {
                ResOutput.ErrorEventHandler(ex.Message);
                return BadRequest(ResOutput);
            }
        }
    }
}
