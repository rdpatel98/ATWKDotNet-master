using SHABS.BUSINESSSERVICES;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace ITEK.WEBAPI.ActionFilters
{
    public class AuthorizationRequiredAttribute : ActionFilterAttribute
    {
        private const string Token = "Token";

        public override void OnActionExecuting(HttpActionContext filterContext)
        {
            //  Get API key provider
            var provider = new TokenServices();
            if (System.Configuration.ConfigurationManager.AppSettings["AuthorizationOn"].Trim() == "1")
            {

                if (filterContext.Request.Headers.Contains(Token))
                {
                    var tokenValue = filterContext.Request.Headers.GetValues(Token).First();

                    if (tokenValue == System.Configuration.ConfigurationManager.AppSettings["ServerAccessKey"])
                    {
                        //all server access key
                    }
                    else
                        if (provider != null && !provider.ValidateToken(tokenValue))
                        {
                            var responseMessage = new HttpResponseMessage(HttpStatusCode.Unauthorized) { ReasonPhrase = "Invalid Request" };
                            filterContext.Response = responseMessage;
                        }
                }
                else
                {
                    filterContext.Response = new HttpResponseMessage(HttpStatusCode.Unauthorized);
                }
            }

            base.OnActionExecuting(filterContext);

        }
    }
}