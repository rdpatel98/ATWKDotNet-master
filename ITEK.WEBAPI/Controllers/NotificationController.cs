//using AttributeRouting.Web.Http;
using ITEK.WEBAPI.ActionFilters;
using ITEK.WEBAPI.Filters;
using SHABS.MODELS;
using System.Collections.Generic;
using System.Web.Http;

namespace ITEK.WEBAPI.Controllers
{
    [ApiAuthenticationFilter]
    public class NotificationController : ApiController
    {

        [Route("GetUserNotification")]
        [Route("GetUserNotification/{MailId}")]
        [HttpPost]
        //[AuthorizationRequired]
        public List<NotificaitonModel> UserNotifications(string MailId)
        {
            return NotificaitonModel.GetUserNotification(MailId);
        }


        [Route("GetScollerNotification")]
        [Route("GetScollerNotification/{MailId}")]
        [HttpPost]
        //[AuthorizationRequired]
        public List<NotificaitonModel> ScollerNotifications(string MailId)
        {
            return NotificaitonModel.GetScollerNotification(MailId);
        }

        [HttpPost]
        [Route("GetModiatorNotification")]
        //[AuthorizationRequired]
        public List<NotificaitonModel> ModiatorNotifications()
        {
            return NotificaitonModel.GetModiatorNotification();
        }


        //[POST("GetUserNotificationCount")]
        //[POST("GetUserNotificationCount/{MailId}")]
        [HttpPost]
        [Route("GetUserNotificationCount")]
        //[AuthorizationRequired]
        public List<string> GetUserNotificationCount(string MailId)
        {
            return NotificaitonModel.GetUserNotificationCount(MailId);
        }
    }
}
