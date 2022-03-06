using ITEK.WEBAPI.Filters;
using SHABS.MODELS;
using System.Web.Http;

namespace ITEK.WEBAPI.Controllers
{
    public class FeedbackController : ApiController
    {
        //[AuthorizationRequired]
        [Route("Feeedback/Send")]
        [ApiAuthenticationFilter]
        [HttpPost]
        public string SendFeedback(FeedbackModel model)
        {
            return model.Save();
        }


    }
}
