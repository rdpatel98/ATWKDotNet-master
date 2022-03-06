//using AttributeRouting.Web.Http;
using ITEK.WEBAPI.ActionFilters;
using ITEK.WEBAPI.Filters;
using SHABS.BUSINESSSERVICES;
using SHABS.DB;
using SHABS.MODELS;
using System;
using System.Configuration;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Configuration;
using System.Web.Http;

namespace ITEK.WEBAPI.Controllers
{
    // [ApiAuthenticationFilter]
    public class AuthenticateController : ApiController
    {
        #region Private variable.

        private readonly ITokenServices _tokenServices;

        #endregion

        #region Public Constructor

        /// <summary>
        /// Public constructor to initialize product service instance
        /// </summary>
        public AuthenticateController()
        {
            _tokenServices = new TokenServices();// tokenServices;
        }

        #endregion

        /// <summary>
        /// Authenticates user and returns token with expiry.
        /// </summary>
        /// <returns></returns>
        [Route("authenticate/{appVersion?}")]
        [HttpPost]
        [ApiAuthenticationFilter]
        public async Task<HttpResponseMessage> AuthenticateAsync(string appVersion = "")
        {
            //check for app version
            if (string.IsNullOrEmpty(appVersion))
            {
                return Request.CreateResponse(HttpStatusCode.OK, "Old App Version");
            }
            else if (Convert.ToInt32(appVersion) < Convert.ToInt32(WebConfigurationManager.AppSettings["AppVersionSupport"]))
            {
                return Request.CreateResponse(HttpStatusCode.OK, "Old App Version");
            }
            if (System.Threading.Thread.CurrentPrincipal != null && System.Threading.Thread.CurrentPrincipal.Identity.IsAuthenticated)
            {
                var basicAuthenticationIdentity = System.Threading.Thread.CurrentPrincipal.Identity as BasicAuthenticationIdentity;
                if (basicAuthenticationIdentity != null)
                {
                    var userId = basicAuthenticationIdentity.UserId;
                    FirebaseDBHelper firebaseDB = new FirebaseDBHelper();
                   await firebaseDB.UpdateUserStatusAsync(userId.ToString(), "ONLINE");
                    return GetAuthToken(userId);
                }
            }
            return Request.CreateResponse(HttpStatusCode.OK, "Login Failed");
        }


        [Route("RegisterForNotification")]
        [HttpPost]
        //[AuthorizationRequired]
        [ApiAuthenticationFilter]
        public string RegisterForNotification(string userID, string registrationID)
        {
            if (string.IsNullOrEmpty(registrationID))
            {
                return "ERROR";
            }
            return UserDetailsModel.StoreRegistrationID(userID, registrationID);
        }

        /// <summary>
        /// Returns auth token for the validated user.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        ///  [ApiAuthenticationFilter]
        [ApiAuthenticationFilter]
        private HttpResponseMessage GetAuthToken(int userId)
        {
            var token = _tokenServices.GenerateToken(userId);
            var response = Request.CreateResponse(HttpStatusCode.OK, "Authorized");
            response.Headers.Add("UserID", userId.ToString());
            response.Headers.Add("Token", token.AuthToken);
            response.Headers.Add("TokenExpiry", ConfigurationManager.AppSettings["AuthTokenExpiry"]);
            response.Headers.Add("Access-Control-Expose-Headers", "Token,TokenExpiry");
            return response;
        }
    }
}
