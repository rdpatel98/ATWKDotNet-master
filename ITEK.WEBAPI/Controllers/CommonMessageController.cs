//using AttributeRouting.Web.Http;
using SHABS.MODELS;
using System.Collections.Generic;
using System.Web.Http;

namespace ITEK.WEBAPI.Controllers
{

    public class CommonMessageController : ApiController
    {
        //[AuthorizationRequired]
        //[POST("CommonMessage/Send")]
        [Route("CommonMessage/Send")]
        [HttpPost]
        public string SendCommonMessage(CommonMessageModel model)
        {
            return model.Save();
        }

        //[GET("CommonMessage/All")]
        [Route("CommonMessage/All")]
        [HttpGet]
        public List<CommonMessageModel> GetAllMessages()
        {
            CommonMessageModel model = new CommonMessageModel();
            return model.GetAllMessages();
        }


    }
}
