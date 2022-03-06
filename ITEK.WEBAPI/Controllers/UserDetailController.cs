//using AttributeRouting.Web.Http;
using ITEK.WEBAPI.ActionFilters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using SHABS.MODELS;
using SHABS.BUSINESSSERVICES;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using System.Web;
using ITEK.WEBAPI.Filters;
using Firebase.Database;
using Firebase.Database.Query;
using SHABS.DB;
using System.Threading;

namespace ITEK.WEBAPI.Controllers
{
    public class UserDetailController : ApiController
    {
        public UserDetailController()
        {

        }
        //[Route("authenticate/9")]
        //[HttpPost]
        //public async Task<string> Authenticate(string username , string password)//testing purpose only!
        //{
        //    IUserServices userServices = new UserServices();
        //    int result = userServices.Authenticate(username , password);
        //    if(result > 0)
        //    {
        //        return "SUCCESS";
        //    }
        //    else
        //    {
        //        return "ERROR";
        //    }
        //}
        [Route("Register")]
        [HttpPost]
        // ////[AuthorizationRequired]
        public async Task<string> RegisterUser(UserDetailsModel model)
        {
            RegistrationResponse res = new RegistrationResponse();
            if (string.IsNullOrEmpty(model.ImageText))
            {
                IUserServices userService = new UserServices();
                res = userService.RegisterUser(model);
                return res.Status;
            }
            else
            {
                res = model.RegisterUserWithImage();
                if (res.Status == "SUCCESS")
                {
                    await SaveRegisterDataInFirebase(model, res.UserID, res.ImageID);
                    return res.Status;
                }
                else
                {
                    return res.Status;
                }

            }
        }
        //[Route("InsertUser")]
        //[HttpPost]
        //// ////[AuthorizationRequired]
        //public async void InsertUser(FirebaseUser user)
        //{
        //    int lastkey = 0;
        //    var firebase = new FirebaseClient("https://atwk-testapp.firebaseio.com/");
        //    var lastnode = await firebase
        //      .Child("atwk_user_info")
        //      .OrderByKey()
        //      .LimitToLast(1)
        //      .OnceAsync<FirebaseUser>();
        //    Console.WriteLine("test");
        //    foreach (var k in lastnode)
        //    {
        //        lastkey = Convert.ToInt32(k.Key);
        //    }

        //    lastkey = lastkey + 1;
        //    user.UserID = lastkey.ToString();
        //     await firebase
        //        .Child("atwk_user_info")
        //        .Child(lastkey.ToString())
        //        .PutAsync(user);
        //    Console.WriteLine("test");
        //}
        //public static async void SaveDataInFirebase(DataTable savedDT)
        //{
        private async Task SaveRegisterDataInFirebase(UserDetailsModel temp, string usrID, string imgID)
        {
            FirebaseUser fbusr = new FirebaseUser();
            fbusr.ImageID = usrID;
            fbusr.UserID = imgID;


            fbusr.UserType = temp.UserModel.UserType;
            fbusr.Details = temp.Details;
            fbusr.Gender = temp.Gender;
            fbusr.Language = temp.Language;
            fbusr.LastOnlineTime = temp.LastOnlineTime;
            fbusr.Nationality = temp.Nationality;
            fbusr.Name = temp.Name;
            fbusr.SpecialisationIn = temp.SpecialisationIn;
            fbusr.StudiesAt = temp.StudiesAt;
            fbusr.UserLoginStatus = temp.UserModel.UserLoginStatus;
            fbusr.UserName = temp.UserModel.UserName;
            fbusr.UserTimeZone = "";
            fbusr.Location = temp.Location;

            var firebase = new FirebaseClient("https://atwk-testapp.firebaseio.com/");
            await firebase
                .Child("atwk_user_info")
                .Child(usrID)
                .PutAsync(fbusr);
        }

        [Route("UpdateProfile")]
        //[AuthorizationRequired]//
        [HttpPost]
        [ApiAuthenticationFilter]
        public async Task<string> UpdateProfileAsync(UserDetailsModel model)
        {
            if (model != null)
            {
                IUserServices userService = new UserServices();
                string result = await model.UpdateProfileAsync();
                return result;
            }
            return "ERROR";
        }

        [Route("UpdateProfileImage")]
        [HttpPost]
        [ApiAuthenticationFilter]
        public async Task<string> UpdateProfileImageAsync(UserDetailsModel model)
        {
            return await model.UpdateProfileImageAsync();
        }

        //[Route("Activate/{autoID}/{identifier}")]
        //public string GetActivateUser(string autoID, string identifier)
        //{
        //    IUserServices userService = new UserServices();
        //    return userService.ActivateUser(identifier,autoID); 
        //}

        [Route("Activate/{autoID}/{identifier}")]
        [HttpGet]
        public async Task<HttpResponseMessage> GetActivateUserAsync(string autoID, string identifier)
        {
            IUserServices userService = new UserServices();
            var response = new HttpResponseMessage();

            if (userService.ActivateUser(identifier, autoID) == "SUCCESS")
            {
                response.Content = new StringContent("<html><body>User activated successfully</body></html>");
            }
            else
            {
                response.Content = new StringContent("<html><body>User activation failed.Please  try in sometime.</body></html>");
            }
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html");
            return response;
        }

        [Route("DeActivate/{identifier}")]
        [HttpPost]
        //[AuthorizationRequired]
        [ApiAuthenticationFilter]
        public async Task<string> DeActivateUserAsync(string identifier)
        {

            IUserServices userService = new UserServices();
            return await userService.DeActivateUserAsync(identifier);
        }

        [Route("ActivateMVC/{identifier}")]
        [HttpGet]
        //[AuthorizationRequired]
        [ApiAuthenticationFilter]
        public async Task<string> GetActivateMVCUserAsync(string identifier)
        {
            //IUserServices userService = new UserServices();
            return await UserDetailsModel.UpdateUserStatusAsync(identifier, "ACTV");
        }

        [Route("DeActivateMVC/{identifier}")]
        [HttpGet]
        //[AuthorizationRequired]
        [ApiAuthenticationFilter]
        public async Task<string> GetDeActivateMVCUserAsync(string identifier)
        {

            //IUserServices userService = new UserServices();
            return await UserDetailsModel.UpdateUserStatusAsync(identifier, "IACT");
        }

        [Route("ReActivate")]
        [HttpPost]
        public string ReActivateUser(string username)
        {
            return UserDetailsModel.ReActivateUser(username);
            //IUserServices userService = new UserServices();
            //return userService.DeActivateUser(identifier);
        }

        ////[AuthorizationRequired]
        [Route("Userdetails/{identifier}")]
        [HttpGet]
        [ApiAuthenticationFilter]
        //[AuthorizationRequired]
        public async Task<UserDetailsModel> GetUserDetailsAsync(string identifier)
        {
            IUserServices userService = new UserServices();
            return await userService.GetUserDetailsAsync(identifier);
        }
        

        ////[AuthorizationRequired]
        //[Route("ChangePassword")]
        //[HttpPost]
        //[AuthorizationRequired]
        //public string ChangePassword(UserModel model)
        //{
        //    return model.ChangePassword(0);
        //}

        [Route("ChangePassword")]
        [HttpPost]
        //[AuthorizationRequired]
        [ApiAuthenticationFilter]
        public string ChangePassword2(UserModel model)
        {
            return model.ChangePassword(1);
        }

        [Route("ForgotPassword")]
        [HttpGet]
        public string GetForgotPassword(string username)
        {

            return UserModel.ForgotPassword(username);
        }

        [Route("Status")]
        [HttpPost]
        //[AuthorizationRequired]
        [ApiAuthenticationFilter]
        public async Task<string> StatusIndicatorAsync(string userID, string Status)
        {
            FirebaseDBHelper firebaseDB = new FirebaseDBHelper();
            int result = await firebaseDB.UpdateUserStatusAsync(userID, Status);            
            if (result > 0)
            {
                return "SUCCESS";
            }
            else
            {
                return "ERROR";
            }
        }

        [Route("ByeBye")]
        [HttpPost]
        ////[AuthorizationRequired]
        public async Task<string> LogoutAsync(string userID)
        {
            FirebaseDBHelper firebaseDB = new FirebaseDBHelper();
            return await UserDetailsModel.UpdateUserStatusAsync(userID, "OFFLINE");
        }
        ////http://hintdesk.com/google-cloud-messaging-asp-net-web-api-and-android-client/
        [Route("Alims")]
        [HttpGet]
        //[AuthorizationRequired]
        [ApiAuthenticationFilter]
        public List<UserDetailsModel> GetAllAlims()
        {
            return UserDetailsModel.AllALims();
        }

        [Route("AlimsByLocation")]
        [HttpGet]
        //[AuthorizationRequired]
        [ApiAuthenticationFilter]
        public List<UserDetailsModel> GetAllAlimsByLocation(string location)
        {
            return UserDetailsModel.AlimsByLocation(location);
        }

        [Route("Users")]
        [HttpGet]
        //[AuthorizationRequired]
        [ApiAuthenticationFilter]
        public List<UserDetailsModel> GetAllUsers()
        {
            return UserDetailsModel.AllUsers();
        }

        [Route("ChatHistoryUsers")]
        [HttpGet]
        //[AuthorizationRequired]
        [ApiAuthenticationFilter]
        public List<IBase> GetChatHistoryUsers(string userID)
        {
            return UserDetailsModel.GetChatHistoryUsers(userID);
        }

        [Route("IncomingMessageUsers")]
        [HttpGet]
        //[AuthorizationRequired]
        [ApiAuthenticationFilter]
        public List<IBase> GetIncomingMessageUsers(string userID)
        {
            return UserDetailsModel.GetIncomingMessageUsers(userID);
        }

        [Route("OutgoingMessageUsers")]
        [HttpGet]
        //[AuthorizationRequired]
        [ApiAuthenticationFilter]
        public List<UserDetailsModel> GetOutgoingMessageUsers(string userID)
        {
            return UserDetailsModel.GetOutgoingMessageUsers(userID);
        }

        //[AuthorizationRequired]
        [Route("UnansweredHistoryUsers")]
        [HttpGet]
        [ApiAuthenticationFilter]
        public List<IBase> GetUnansweredHistoryUsers()
        {
            return UserDetailsModel.GetUnansweredQueryUsers();
        }

        /// <summary>
        /// Returns all the user list
        /// </summary>
        /// <returns></returns>
        [Route("Users2")]
        [HttpGet]
        [ApiAuthenticationFilter]
        //[AuthorizationRequired]
        public List<UserDetailsModel> GetAllUsers2()
        {
            return UserDetailsModel.AllAccessors("USER");
        }

        /// <summary>
        /// Returns all the scholar list
        /// </summary>
        /// <returns></returns>
        [Route("Scholars2")]
        [HttpGet]
        //[AuthorizationRequired]
        [ApiAuthenticationFilter]
        public List<UserDetailsModel> GetAllScholars2()
        {
            return UserDetailsModel.AllAccessors("ALIM");
        }

        [Route("Moderators2")]
        [HttpGet]
        //[AuthorizationRequired]
        [ApiAuthenticationFilter]
        public List<UserDetailsModel> GetAllModerators2()
        {
            return UserDetailsModel.AllAccessors("MODERATOR");
        }

        [Route("DeleteUser")]
        [HttpGet]
        //[AuthorizationRequired]
        [ApiAuthenticationFilter]
        public string GetDeleteUser(string identifier)
        {
            return UserDetailsModel.DeleteUser(identifier);
        }

        [Route("Home")]
        [HttpGet]
        //[AuthorizationRequired]
        [ApiAuthenticationFilter]
        public dynamic GetHomePageCount()
        {

            return UserDetailsModel.GetHomePageCount();
        }

        //[AuthorizationRequired]
        [ApiAuthenticationFilter]
        [Route("UpdateTypicallyRepliesIn")]
        [HttpPost]
        public string UpdateTypicallyRepliesIn(UserDetailsModel model)
        {
            return model.UpdateTypicallyRepliesIn();
        }

        //[AuthorizationRequired]
        [Route("GetPendingQueryAlimsWithCount")]
        [ApiAuthenticationFilter]
        [HttpGet]
        public List<IBase> GetPendingQueryAlimsWithCount()
        {
            return UserDetailsModel.GetPendingQueryAlimsWithCount();
        }

        //[AuthorizationRequired]
        [Route("PendingQueryWCByAlim")]
        [ApiAuthenticationFilter]
        [HttpPost]
        public List<IBase> PendingQueryWCByAlim(string scholarUsername)
        {
            return UserDetailsModel.GetPendingQueryWCByAlim(scholarUsername);
        }

        /// <summary>
        /// Author : Zainab Rizvi
        /// Creation Date : 15 may 2019
        /// Purpose : This Method will return alim userid with their question avg response time.
        /// </summary>
        /// <returns>list of response time</returns>
        //[GET("GetMessageResponseTime")]
        [Route("GetMessageResponseTime")]
        [ApiAuthenticationFilter]
        [HttpGet]
        public List<IResponseTimeModel> GetMessageResponseTime()
        {
            return UserDetailsModel.GetMessageResponseTime();
        }

        //  public List<UserDetailsModel> Get


        // [Route("UploadImage2")]
        //public async Task<HttpResponseMessage> ImageUpload2(string userid)
        //{
        //    if (!Request.Content.IsMimeMultipartContent())
        //    {
        //        throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
        //    }
        //    CustomMultipartMemoryStreamProvider provider = new CustomMultipartMemoryStreamProvider();// (fileSaveLocation);

        //    byte[] x =await Request.Content.ReadAsByteArrayAsync();
        //    List<string> files = new List<string>();
        //    return Request.CreateResponse(HttpStatusCode.OK);
        //}

        // [Route("UploadImage")]
        //public async Task<HttpResponseMessage> ImageUpload(string userid)
        //{
        //    if (!Request.Content.IsMimeMultipartContent())
        //    {
        //        throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
        //    }

        //    string fileSaveLocation = HttpContext.Current.Server.MapPath("~/App_Data");
        //    CustomMultipartFormDataStreamProvider provider = new CustomMultipartFormDataStreamProvider(fileSaveLocation);
        //    List<string> files = new List<string>();
        //    try
        //    {
        //        // Read all contents of multipart message into CustomMultipartFormDataStreamProvider.
        //        await Request.Content.ReadAsMultipartAsync(provider);

        //        foreach (MultipartFileData file in provider.FileData)
        //        {
        //            files.Add(System.IO.Path.GetFileName(file.LocalFileName));
        //        }

        //        // Send OK Response along with saved file names to the client.
        //        return Request.CreateResponse(HttpStatusCode.OK, files);
        //    }
        //    catch (System.Exception e)
        //    {
        //        return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
        //    }
        //}
        // 
    }



    //public class CustomMultipartMemoryStreamProvider : MultipartMemoryStreamProvider
    //{
    //   // public override 
    //}


    //public class CustomMultipartFormDataStreamProvider : MultipartFormDataStreamProvider
    //{
    //    public CustomMultipartFormDataStreamProvider(string path) : base(path) { }

    //    public override string GetLocalFileName(HttpContentHeaders headers)
    //    {
    //        return headers.ContentDisposition.FileName.Replace("\"", string.Empty);
    //    }
    //}
}
