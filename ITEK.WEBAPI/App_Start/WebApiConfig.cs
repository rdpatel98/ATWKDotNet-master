using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Routing;
using System.Net.Http.Formatting;
using System.Web.Http.Cors;

namespace ITEK.WEBAPI
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            //config.Routes.MapHttpRoute(
            //    name: "DefaultApi",
            //    routeTemplate: "api/{controller}/{id}",
            //    defaults: new { id = RouteParameter.Optional }
            //);

            //config.Routes.MapHttpRoute(
            //name: "ActionBased",
            //routeTemplate: "api/{controller}/{action}/{id}",
            //defaults: new { id = RouteParameter.Optional }
            //);

            //        config.Routes.MapHttpRoute(
            //name: "DefaultApiWithExtension1",
            //routeTemplate: "api/{controller}/{action}.{ext}",
            //defaults: null,
            //constraints: new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });


            //config.Formatters.XmlFormatter.AddUriPathExtensionMapping("xml", "application/xml");
            //config.Formatters.JsonFormatter.AddUriPathExtensionMapping("json", "application/json");
            var cors = new EnableCorsAttribute("*", "*", "*");
            config.EnableCors(cors);
            config.MapHttpAttributeRoutes();

        }
    }
}
