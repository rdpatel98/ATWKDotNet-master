//using AttributeRouting.Web.Http;
using Firebase.Database;
using Firebase.Database.Query;
using ITEK.WEBAPI.ActionFilters;
using ITEK.WEBAPI.Filters;
using ITEK.WEBAPI.Models;
using Newtonsoft.Json;
using SHABS.DB;
using SHABS.MODELS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Threading.Tasks;
using System.Web.Http;
using static SHABS.MODELS.BaseMessageModel;

namespace ITEK.WEBAPI.Controllers
{

    public class MessagesController : ApiController
    {
        [Route("Test")]
        [HttpPost]
        [ApiAuthenticationFilter]
        //[AuthorizationRequired]
        public void Test(MessageModel model)
        {
            //GCMHelper.PushNotificationByUsername(model.Text, model.To);
        }

        [Route("Message/Shabie")]
        [HttpGet]
        [ApiAuthenticationFilter]

        // [AuthorizationRequired]
        public string GetShabie()
        {
            //GCMHelper.PushNotificationByUsername(model.Text, model.To);
            PushNotification pushNotification = new PushNotification();

            // pushNotification.SendNotification("Test body sent", "Sample", "c5Cf2EHXT4I:APA91bEyzCH8Jr4QaxyPKr4_4yvH1okY6xi6ZItvoRgJj0CILraW_bAXnNZtcOj6j2_fi32sS4Yy5TFDVcJY-3ZDohfs-6bNDv3ISF2dSoOvdXSs7skZN_1NVmp0zsS5vuna557a86A9".Split(',').ToList());
            return "test";
        }
        [Route("Message/TestPost")]
        [HttpPost]
        [AllowAnonymous]
        public async void TestPostFirebase(string name, int age)
        {
            var info = new { Name = name, Age = age };
            var firebase = new FirebaseClient("YourFirebaseRealtimeDatabaseURL");
            await firebase.Child("Test").PostAsync(info);
        }
        [Route("Message/GetUserMessges")]
        [HttpPost]
        [AllowAnonymous]
        public async Task<string> GetUserMessages(int userID)
        {
            OleDBHelper oleDBHelper = new OleDBHelper();
            return oleDBHelper.GetUserData(userID);
        }
        [Route("Message/GetUserMessgesBtw")]
        [HttpPost]
        [AllowAnonymous]
        public async Task<string> GetUserMessages(string me, string to, string subject)
        {
            OleDBHelper oleDBHelper = new OleDBHelper();
            return JsonConvert.SerializeObject(oleDBHelper.GetUserDataBtwAsync(me, to, subject, Guid.NewGuid().ToString()));
        }
        //[AuthorizationRequired]
        [Route("Message/Send")]
        [HttpPost]
        [ApiAuthenticationFilter]
        public async Task<MessageModel> SendMessageAsync(MessageModel model)
        {
            OleDBHelper oleDBHelper = new OleDBHelper();

            var guid = Guid.NewGuid().ToString();
            FirebaseDBHelper firebaseDBHelper = new FirebaseDBHelper();
            await model.SaveMessageAsync(guid);

            MessageModel objModelResult = await model.GetJustSentMessageAsync(guid);
            await firebaseDBHelper.WriteLog(guid, "objModelResult", JsonConvert.SerializeObject(objModelResult));
            model.MessageID = objModelResult.MessageID;
            model.FromUserID = objModelResult.FromUserID;
            model.ToUserID = objModelResult.ToUserID;
            model.FromUserType = objModelResult.FromUserType;
            var result = await model.SaveMessageAsync(model);
            await firebaseDBHelper.WriteLog(guid, "SaveMessageInFirebase", JsonConvert.SerializeObject(result));
            await firebaseDBHelper.WriteLog(guid, "Model", JsonConvert.SerializeObject(model));
            if (model.FromUserType == "USER")
            {
                await firebaseDBHelper.AddMessageToModeratorReview(SHABS.MODELS.ObjectToDictionaryHelper.ToDictionary(model));
            }
            if (result == "SUCCESS" && model.MessageStatus == "APPROVE")
            {
                objModelResult.FromUserType = "ALIM";
                string registrationID = UserDetailsModel.GetRegistrationID(model.To, "1");
                if (registrationID != string.Empty)
                {
                    PushNotification obj = new PushNotification();
                    NotificaitonModel notificationmodel = new NotificaitonModel();
                    notificationmodel = NotificaitonModel.GetUserPushNotificationDetails(model.MessageReplyID);
                    notificationmodel.ID = model.MessageID;
                    notificationmodel.FromUserType = objModelResult.FromUserType;
                    if (notificationmodel != null)
                    {
                        obj.SendUserNotification(notificationmodel, registrationID);
                    }
                }
            }
            else if (result == "SUCCESS")
            {
                objModelResult.FromUserType = "USER";
                PushNotification obj = new PushNotification();
                string registrationId = NotificaitonModel.GetAllModeators();
                NotificaitonModel notificationmodel = new NotificaitonModel();
                notificationmodel = NotificaitonModel.GeModelPushNotificationDetails(model.From, model.To);
                if (registrationId != null)
                {
                    notificationmodel.ayatollah = model.ayatollah;
                    notificationmodel.Subject = model.Subject;
                    notificationmodel.Text = model.Text;
                    notificationmodel.SendTo = model.To;
                    notificationmodel.SentFrom = model.From;
                    notificationmodel.ID = objModelResult.MessageID;
                    notificationmodel.FromUserType = objModelResult.FromUserType;

                    string[] regids = registrationId.Split(',');
                    foreach (string Regid in regids)
                    {
                        obj.SendModelNotification(notificationmodel, Regid);
                    }
                }
            }
            if (result == "SUCCESS")
            {
                //MessageModel objModelResult = model.GetJustSentMessage();
                return objModelResult;
            }
            else
            {
                model.MessageID = "";
                return model;
            }
            //      var resultDict = result.GetType()
            //.GetProperties(BindingFlags.Instance | BindingFlags.Public)
            //     .ToDictionary(prop => prop.Name, prop => (string)prop.GetValue(result, null));
            //      var modelDict = model.GetType()
            //.GetProperties(BindingFlags.Instance | BindingFlags.Public)
            //     .ToDictionary(prop => prop.Name, prop => (string)prop.GetValue(model, null));
            //      if (model.FromUserType == "ALIM")
            //      {
            //          await firebaseDBHelper.UpdateLatestSubject(modelDict , model.From);
            //          await firebaseDBHelper.UpdateLatestSubject(modelDict, model.To);
            //      }
            //      else
            //      {
            //          await firebaseDBHelper.UpdateLatestSubject(modelDict, model.From);
            //      }
            //      MessageModel objModelResult = (await firebaseDBHelper.GetMessageDetails(result)).ToObject<MessageModel>();
            //      if (model.MessageStatus == "APPROVE")
            //      {
            //          objModelResult.FromUserType = "ALIM";
            //          string registrationID = UserDetailsModel.GetRegistrationID(model.To, "1");
            //          if (registrationID != string.Empty)
            //          {
            //              PushNotification obj = new PushNotification();
            //              NotificaitonModel notificationmodel = new NotificaitonModel();
            //              notificationmodel = NotificaitonModel.GetUserPushNotificationDetails(model.MessageReplyID);
            //              notificationmodel.ID = model.MessageID;
            //              notificationmodel.FromUserType = objModelResult.FromUserType;
            //              if (notificationmodel != null)
            //              {
            //                  obj.SendUserNotification(notificationmodel, registrationID);
            //              }
            //          }
            //      }
            //      else
            //      {
            //          objModelResult.FromUserType = "USER";
            //          PushNotification obj = new PushNotification();
            //          string registrationId = NotificaitonModel.GetAllModeators();
            //          NotificaitonModel notificationmodel = new NotificaitonModel();
            //          notificationmodel = NotificaitonModel.GeModelPushNotificationDetails(model.From, model.To);
            //          if (registrationId != null)
            //          {
            //              notificationmodel.ayatollah = model.ayatollah;
            //              notificationmodel.Subject = model.Subject;
            //              notificationmodel.Text = model.Text;
            //              notificationmodel.SendTo = model.To;
            //              notificationmodel.SentFrom = model.From;
            //              notificationmodel.ID = objModelResult.MessageID;
            //              notificationmodel.FromUserType = objModelResult.FromUserType;

            //              string[] regids = registrationId.Split(',');
            //              foreach (string Regid in regids)
            //              {
            //                  obj.SendModelNotification(notificationmodel, Regid);
            //              }
            //          }
            //      }           
            //          return objModelResult;                       
        }

        [Route("Message/Count/{Direction}")]
        [HttpGet]
        [ApiAuthenticationFilter]
        //[AuthorizationRequired]
        public MessageCountModel GetMessageCountByUsername(string Direction, string username)
        {
            return MessageCountModel.GetMessageCount(Direction, username);
        }

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


        // //[AuthorizationRequired]
        [Route("Message/MyDecision")]
        [HttpPost]
        [ApiAuthenticationFilter]
        //[AuthorizationRequired]
        public async Task<string> ApproveRejectMessage(MessageModel model)
        {
            var result = model.UpdateMessage();
            if (result == "SUCCESS" && model.MessageStatus == "APPROVE")
            {
                //if(model.UserID == "1000")
                //{
                //GCMHelper.PushNotificationByUsername(model, model.To);
                string registrationID = UserDetailsModel.GetRegistrationScollerID(model.MessageID);
                if (registrationID != string.Empty)
                {
                    PushNotification obj = new PushNotification();
                    NotificaitonModel notificationmodel = new NotificaitonModel();
                    notificationmodel = NotificaitonModel.GetSchollerPushNotificationDetails(model.MessageID);
                    notificationmodel.ID = model.MessageID;
                    if (notificationmodel != null)
                    {
                        obj.SendScholerNotification(notificationmodel, registrationID);
                    }
                }
                //}
            }

            if (model.MessageStatus == "APPROVE" || model.MessageStatus == "REJECT")
            {
                await UpdateChatMessageStatus(model);
            }
            return result;
        }

        private async Task<int> UpdateChatMessageStatus(MessageModel model)
        {
            try
            {
                model.MessageStatus = model.MessageStatus;
                await model.UpdateChatMessageAsync(model);
                return 1;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        //  //[AuthorizationRequired]
        [Route("Message/MarkRead")]
        [HttpPost]
        [ApiAuthenticationFilter]
        //[AuthorizationRequired]
        public string MarkRead(MessageModel model)
        {
            return model.MarkAsRead();
        }

        [Route("Message/MarkMultipleRead")]
        [HttpPost]
        [ApiAuthenticationFilter]
        //[AuthorizationRequired]
        public async Task<string> MarkMultipleRead(MessageModel model)
        {
            await MarkAllMessageAsRead(model);
            await MarkLatestMessageAsRead(model);
            return model.MarkMultipleRead();
        }
        private async Task<int> MarkAllMessageAsRead(MessageModel model)
        {
            try
            {
                FirebaseDBHelper firebaseDBHelper = new FirebaseDBHelper();
                Dictionary<string, string> firebaseParameters = new Dictionary<string, string>
            {
                { "FromUserType", model.FromUserType },
                { "UserID", model.FromUserID },
                { "ToID", model.ToUserID },
                { "Subject", model.Subject },
                { "Ayatollah", model.ayatollah },
                { "MessageID", model.MessageID },
                { "FromUserID", model.FromUserID },
                { "ToUserID", model.ToUserID }
            };

                var allMessages = await firebaseDBHelper.GetAllChatMessages(firebaseParameters);

                if (allMessages != null)
                {

                    var allMessageDataJson = JsonConvert.SerializeObject(allMessages);
                    var allMsgsData = JsonConvert.DeserializeObject<List<Root>>(allMessageDataJson);

                    foreach (var itm in allMsgsData)
                    {
                        if (itm.Value.IsRead != "Y")
                        {
                            itm.Value.IsRead = "Y";
                            await model.UpdateChatMessageAsync(itm.Value);
                        }
                    }
                }

                return 1;
            }
            catch (Exception)
            {
                return 0;
            }
        }


        private async Task<string> MarkLatestMessageAsRead(MessageModel model)
        {
            FirebaseDBHelper firebaseDBHelper = new FirebaseDBHelper();
            var result = "";
            Dictionary<string, string> firebaseParameters = new Dictionary<string, string>
            {
                { "FromUserType", model.FromUserType },
                { "UserID", model.FromUserID },
                { "ToID", model.ToUserID },
                { "Subject", model.Subject },
                { "Ayatollah", model.ayatollah },
                { "MessageID", model.MessageID },
                { "FromUserID", model.FromUserID },
                { "ToUserID", model.ToUserID }
            };
            var latestMessageDetails = await firebaseDBHelper.GetLatestMessageDetails(firebaseParameters);

            if (latestMessageDetails != null)
            {
                var latestMessageDataJson = JsonConvert.SerializeObject(latestMessageDetails);
                var latestMsgData = JsonConvert.DeserializeObject<MessageModel>(latestMessageDataJson);
                if (latestMsgData.IsRead != "Y")
                {
                    latestMsgData.IsRead = "Y";
                    result = await model.UpdateLatestMessageAsync(latestMsgData);
                }

            }


            firebaseParameters["UserID"] = model.ToUserID;
            firebaseParameters["ToID"] = model.FromUserID;
            var latestMessageAnotherUser = await firebaseDBHelper.GetLatestMessageDetails(firebaseParameters);

            if (latestMessageAnotherUser != null)
            {
                var latestMessageDataJson = JsonConvert.SerializeObject(latestMessageAnotherUser);
                var latestMsgData = JsonConvert.DeserializeObject<MessageModel>(latestMessageDataJson);
                if (latestMsgData.IsRead != "Y")
                {
                    latestMsgData.IsRead = "Y";
                    result = await model.UpdateLatestMessageAsync(latestMsgData);
                }

            }
            return result;
        }
        [Route("Message/MarkMultipleIsReply")]
        [HttpPost]
        [ApiAuthenticationFilter]
        //[AuthorizationRequired]
        public async Task<string> UpdateMultipleReply(MessageModel model)
        {
            await MarkAllMessageAsReplied(model);
            await MarkLatestMessageAsReplied(model);
            return model.UpdateMultipleReply();
        }
        private async Task<int> MarkAllMessageAsReplied(MessageModel model)
        {
            try
            {
                FirebaseDBHelper firebaseDBHelper = new FirebaseDBHelper();
                Dictionary<string, string> firebaseParameters = new Dictionary<string, string>
            {
                { "FromUserType", model.FromUserType },
                { "UserID", model.FromUserID },
                { "ToID", model.ToUserID },
                { "Subject", model.Subject },
                { "Ayatollah", model.ayatollah },
                { "MessageID", model.MessageID },
                { "FromUserID", model.FromUserID },
                { "ToUserID", model.ToUserID }
            };

                var allMessages = await firebaseDBHelper.GetAllChatMessages(firebaseParameters);

                if (allMessages != null)
                {

                    var allMessageDataJson = JsonConvert.SerializeObject(allMessages);
                    var allMsgsData = JsonConvert.DeserializeObject<List<Root>>(allMessageDataJson);

                    foreach (var itm in allMsgsData)
                    {
                        if (itm.Value.IsReplied != "Y")
                        {
                            itm.Value.IsReplied = "Y";
                            await model.UpdateChatMessageAsync(itm.Value);
                        }
                    }
                }

                return 1;
            }
            catch (Exception)
            {
                return 0;
            }
        }


        private async Task<string> MarkLatestMessageAsReplied(MessageModel model)
        {
            FirebaseDBHelper firebaseDBHelper = new FirebaseDBHelper();
            var result = "";
            Dictionary<string, string> firebaseParameters = new Dictionary<string, string>
            {
                { "FromUserType", model.FromUserType },
                { "UserID", model.FromUserID },
                { "ToID", model.ToUserID },
                { "Subject", model.Subject },
                { "Ayatollah", model.ayatollah },
                { "MessageID", model.MessageID },
                { "FromUserID", model.FromUserID },
                { "ToUserID", model.ToUserID }
            };
            var latestMessageDetails = await firebaseDBHelper.GetLatestMessageDetails(firebaseParameters);

            if (latestMessageDetails != null)
            {
                var latestMessageDataJson = JsonConvert.SerializeObject(latestMessageDetails);
                var latestMsgData = JsonConvert.DeserializeObject<MessageModel>(latestMessageDataJson);
                if (latestMsgData.IsReplied != "Y")
                {
                    latestMsgData.IsReplied = "Y";
                    result = await model.UpdateLatestMessageAsync(latestMsgData);
                }

            }


            firebaseParameters["UserID"] = model.ToUserID;
            firebaseParameters["ToID"] = model.FromUserID;
            var latestMessageAnotherUser = await firebaseDBHelper.GetLatestMessageDetails(firebaseParameters);

            if (latestMessageAnotherUser != null)
            {
                var latestMessageDataJson = JsonConvert.SerializeObject(latestMessageAnotherUser);
                var latestMsgData = JsonConvert.DeserializeObject<MessageModel>(latestMessageDataJson);
                if (latestMsgData.IsReplied != "Y")
                {
                    latestMsgData.IsReplied = "Y";
                    result = await model.UpdateLatestMessageAsync(latestMsgData);
                }

            }
            return result;
        }


        [Route("Message/MarkIsReplyByMod")]
        [HttpPost]
        [ApiAuthenticationFilter]
        //[AuthorizationRequired]
        public string UpdateIsReplyByMod(String MessageID, String IsReplied, String UserID, String ScholarId)
        {
            if (IsReplied == "Y" || IsReplied == "N")
            {
                String response = MessageModel.UpdateIsReply(MessageID, IsReplied, UserID, ScholarId);
                return response;
            }
            else
            {
                return "Wrong Parameter!";
            }
        }
        //  //[AuthorizationRequired]
        [Route("Message/MessageForApproval")]
        [HttpPost]
        [ApiAuthenticationFilter]
        //[AuthorizationRequired]
        public List<MessageModel> MessageForApproval(string status)
        {
            return MessageModel.GetMessagesForModerator(status);
        }

        //[AuthorizationRequired]
        [Route("Message/Notify")]
        [HttpPost]
        [ApiAuthenticationFilter]
        public async Task<string> NotifyUserAsync(MessageModel model)
        {
            model.MessageStatus = "APPROVE";
            var str = await model.SaveMessageAsync(model);
            if (str != null)
            {
                string registrationID = UserDetailsModel.GetRegistrationID(model.To, "1");
                if (registrationID != string.Empty)
                {
                    PushNotification pushNotification = new PushNotification();
                    NotificaitonModel notificaitonModel = new NotificaitonModel();
                    notificaitonModel = NotificaitonModel.GeModelPushNotificationDetails(model.From, model.To);
                    if (notificaitonModel != null)
                    {
                        notificaitonModel.ayatollah = model.ayatollah;
                        notificaitonModel.Subject = model.Subject;
                        notificaitonModel.Text = model.Text;
                        notificaitonModel.SendTo = model.To;
                        notificaitonModel.SentFrom = model.From;
                        pushNotification.SendNotification(notificaitonModel, registrationID);
                    }
                    return "SUCCESS";
                }
            }
            return "FAIL";
        }

        [Route("Message/AllMessages")]
        [HttpGet]
        [ApiAuthenticationFilter]
        //[AuthorizationRequired]
        public List<MessageModel> GetAllMessages()
        {
            return MessageModel.GetAllMessages();
        }

        [Route("Message/AllMessagesById")]
        //[AuthorizationRequired]
        [HttpGet]
        [ApiAuthenticationFilter]
        public List<MessageModel> GetAllMessagesById(string userID)
        {
            return MessageModel.GetAllMessagesById(userID);
        }

        //  //[AuthorizationRequired]
        [Route("Message/Incoming")]
        //[AuthorizationRequired]
        [HttpPost]
        [ApiAuthenticationFilter]
        public List<MessageModel> MessageByID(string userID)
        {
            return MessageModel.GetIncomingMessages(userID);
        }

        //  //[AuthorizationRequired]
        [Route("Message/OutGoing")]
        [HttpPost]
        [ApiAuthenticationFilter]
        //[AuthorizationRequired]
        public List<MessageModel> OutMessageByID(string userID)
        {
            return MessageModel.GetOutGoingMessages(userID);
        }

        //[AuthorizationRequired]
        [Route("Redirect")]
        [HttpPost]
        [ApiAuthenticationFilter]
        public string RedirectMessage(RedirectMessageModel model)
        {
            string str = model.Redirect();
            if (str == "SUCCESS")
            {
                PushNotification pushNotification = new PushNotification();
                NotificaitonModel notificaitonModel = new NotificaitonModel();
                notificaitonModel = NotificaitonModel.GetSchollerPushNotificationDetails(model.MessageId);
                notificaitonModel.ID = model.MessageId;
                if (notificaitonModel != null && !string.IsNullOrEmpty(notificaitonModel.SendTo))
                {
                    string registrationID = UserDetailsModel.GetRegistrationID(model.FromUserName, "1");
                    string registrationID1 = UserDetailsModel.GetRegistrationID(model.ToId, "0");
                    if (registrationID1 != string.Empty)
                    {
                        pushNotification.SendScholerNotification(notificaitonModel, registrationID1);
                    }
                    if (registrationID != string.Empty)
                    {
                        pushNotification.SendRedirectNotification(notificaitonModel, registrationID);
                    }
                }
            }
            return str;
        }

        [Route("Message/Conversation")]
        [HttpGet]
        [ApiAuthenticationFilter]
        //[AuthorizationRequired]
        public List<MessageModel> GetConversation(string myUsername, string othersUsername)
        {
            return MessageModel.GetConversationMessages(myUsername, othersUsername);
        }

        [Route("Message/ConversationById")]
        //[AuthorizationRequired]
        [HttpGet]
        [ApiAuthenticationFilter]
        public List<MessageModel> GetConversationById(string myUserId, string othersUserId)
        {
            return MessageModel.GetConversationMessagesById(myUserId, othersUserId);
        }


        [Route("Message/Inbox")]
        [HttpGet]
        [ApiAuthenticationFilter]
        //[AuthorizationRequired]
        public List<MessageModel> GetMessageByUsername(string myUsername, string othersUsername)
        {
            return MessageModel.Inbox(myUsername, othersUsername);
        }

        //  //[AuthorizationRequired] http://hintdesk.com/android-upload-files-to-asp-net-web-api-service/
        [Route("Message/OutBox")]
        //[AuthorizationRequired]
        [HttpGet]
        [ApiAuthenticationFilter]
        public List<MessageModel> GetOutMessageByUsername(string myUsername, string othersUsername)
        {
            return MessageModel.OutBox(myUsername, othersUsername);
        }

        [Route("Message/SendVoiceMessage2")]
        [HttpPost]
        [ApiAuthenticationFilter]
        //[AuthorizationRequired]
        public async Task<MessageModel> SendVoiceMessage2Async(MessageModel model)
        {
            byte[] fileBytes = Convert.FromBase64String(model.Text);
            string contenttype = string.Empty;
            switch (System.IO.Path.GetExtension(model.FileName))
            {
                case ".wav":
                    contenttype = "application/wav";
                    break;
                case ".doc":
                    contenttype = "application/vnd.ms-word";
                    break;
                case ".docx":
                    contenttype = "application/vnd.ms-word";
                    break;
                case ".xls":
                    contenttype = "application/vnd.ms-excel";
                    break;
                case ".xlsx":
                    contenttype = "application/vnd.ms-excel";
                    break;
                case ".jpg":
                    contenttype = "image/jpg";
                    break;
                case ".png":
                    contenttype = "image/png";
                    break;
                case ".gif":
                    contenttype = "image/gif";
                    break;
                case ".pdf":
                    contenttype = "application/pdf";
                    break;
                case ".txt":
                    contenttype = "text/plain";
                    break;
                case ".mp3":
                    contenttype = "audio/mpeg";
                    break;
            }
            var guid = Guid.NewGuid().ToString();
            var output = model.SaveVoiceMessage(contenttype, fileBytes);
            MessageModel objModelResult = await model.GetJustSentMessageAsync(guid);
            if (output != "ERROR" && model.MessageStatus == "APPROVE")
            {
                //string val = "bimalchawla89@gmail.com";
                //if (model.To.ToUpper() == val.ToUpper())
                //{
                //GCMHelper.PushNotificationByUsername(model, model.To);
                objModelResult.FromUserType = "ALIM";
                string registrationID = UserDetailsModel.GetRegistrationID(model.To, "1");
                if (registrationID != string.Empty)
                {
                    PushNotification obj = new PushNotification();
                    NotificaitonModel notificationmodel = new NotificaitonModel();
                    notificationmodel = NotificaitonModel.GetUserPushNotificationDetails(model.MessageReplyID);
                    if (notificationmodel != null)
                    {
                        notificationmodel.ID = objModelResult.MessageID;
                        notificationmodel.FromUserType = objModelResult.FromUserType;
                        obj.SendUserNotification(notificationmodel, registrationID);
                    }
                }
                //}
            }
            else if (output != "ERROR")
            {
                //string val="Bhush.patilqa@gmail.com";
                //if (model.To.ToUpper() == val.ToUpper())
                //{
                objModelResult.FromUserType = "USER";
                model.Text = output;
                PushNotification obj = new PushNotification();
                string registrationId = NotificaitonModel.GetAllModeators();
                NotificaitonModel notificationmodel = new NotificaitonModel();
                notificationmodel = NotificaitonModel.GeModelPushNotificationDetails(model.From, model.To);
                if (registrationId != null)
                {
                    notificationmodel.ayatollah = model.ayatollah;
                    notificationmodel.Subject = model.Subject;
                    notificationmodel.Text = model.Text;
                    notificationmodel.SendTo = model.To;
                    notificationmodel.SentFrom = model.From;
                    notificationmodel.IsVoice = "Y";
                    notificationmodel.ID = objModelResult.MessageID;
                    notificationmodel.FromUserType = objModelResult.FromUserType;
                    string[] regids = registrationId.Split(',');
                    foreach (string Regid in regids)
                    {
                        obj.SendModelNotification(notificationmodel, Regid);
                    }
                }
                //}
            }
            // return output;
            if (output != "ERROR")
            {
                //MessageModel objModelResult = model.GetJustSentMessage();
                return objModelResult;
            }
            else
            {
                model.MessageID = "";
                return model;
            }

        }


        [Route("Message/SendDocumentMessage")]
        [HttpPost]
        [ApiAuthenticationFilter]
        //[AuthorizationRequired]
        public async Task<MessageModel> SendDocumentMessageAsync(MessageModel model)
        {
            byte[] fileBytes = Convert.FromBase64String(model.Text.Replace(@"\", string.Empty));
            string contenttype = string.Empty;
            switch (System.IO.Path.GetExtension(model.FileName).ToLower())
            {
                case ".wav":
                    contenttype = "application/wav";
                    break;
                case ".doc":
                    contenttype = "application/vnd.ms-word";
                    break;
                case ".docx":
                    contenttype = "application/vnd.ms-word";
                    break;
                case ".xls":
                    contenttype = "application/vnd.ms-excel";
                    break;
                case ".xlsx":
                    contenttype = "application/vnd.ms-excel";
                    break;
                case ".jpg":
                    contenttype = "image/jpg";
                    break;
                case ".jpeg":
                    contenttype = "image/jpg";
                    break;
                case ".png":
                    contenttype = "image/png";
                    break;
                case ".gif":
                    contenttype = "image/gif";
                    break;
                case ".pdf":
                    contenttype = "application/pdf";
                    break;
                case ".txt":
                    contenttype = "text/plain";
                    break;
                case ".mp3":
                    contenttype = "audio/mpeg";
                    break;
                default:
                    contenttype = "application/file";
                    break;
            }

            var output = model.SaveDocumentMessage(contenttype, fileBytes);
            var guid = Guid.NewGuid().ToString();
            MessageModel objModelResult = await model.GetJustSentMessageAsync(guid);
            if (output != "ERROR" && model.MessageStatus == "APPROVE")
            {
                //string val = "bimalchawla89@gmail.com";
                //if (model.To.ToUpper() == val.ToUpper())
                //{
                //GCMHelper.PushNotificationByUsername(model, model.To);
                objModelResult.FromUserType = "ALIM";
                string registrationID = UserDetailsModel.GetRegistrationID(model.To, "1");
                if (registrationID != string.Empty)
                {
                    PushNotification obj = new PushNotification();
                    NotificaitonModel notificationmodel = new NotificaitonModel();
                    notificationmodel = NotificaitonModel.GetUserPushNotificationDetails(model.MessageReplyID);
                    if (notificationmodel != null)
                    {
                        notificationmodel.FromUserType = objModelResult.FromUserType;
                        notificationmodel.ID = objModelResult.MessageID;
                        obj.SendUserNotification(notificationmodel, registrationID);
                    }
                }
                //}
            }
            else if (output != "ERROR")
            {
                //string val="Bhush.patilqa@gmail.com";
                //if (model.To.ToUpper() == val.ToUpper())
                //{
                objModelResult.FromUserType = "USER";
                model.Text = output;
                PushNotification obj = new PushNotification();
                string registrationId = NotificaitonModel.GetAllModeators();
                NotificaitonModel notificationmodel = new NotificaitonModel();
                notificationmodel = NotificaitonModel.GeModelPushNotificationDetails(model.From, model.To);
                if (registrationId != null)
                {
                    notificationmodel.ayatollah = model.ayatollah;
                    notificationmodel.Subject = model.Subject;
                    notificationmodel.Text = model.Text;
                    notificationmodel.SendTo = model.To;
                    notificationmodel.SentFrom = model.From;
                    notificationmodel.IsVoice = "N";
                    notificationmodel.FromUserType = objModelResult.FromUserType;
                    notificationmodel.ContentType = contenttype;
                    notificationmodel.ID = objModelResult.MessageID;
                    string[] regids = registrationId.Split(',');
                    foreach (string Regid in regids)
                    {
                        obj.SendModelNotification(notificationmodel, Regid);
                    }
                }
                //}
            }

            if (output != "ERROR")
            {
                //MessageModel objModelResult = model.GetJustSentMessage();
                return objModelResult;
            }
            else
            {
                model.MessageID = "";
                return model;
            }
            //return output;

        }

        [Route("Message/SendVoiceMessage")]
        [HttpPost]
        [ApiAuthenticationFilter]
        //[AuthorizationRequired]
        public async Task<MessageModel> SendVoiceMessage()
        {
            if (!Request.Content.IsMimeMultipartContent())
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }

            var root = System.Web.Hosting.HostingEnvironment.MapPath("~/Uploads");
            var provider = new CustomMultipartFormDataStreamProvider(root);
            var result = await Request.Content.ReadAsMultipartAsync(provider);
            if (result.FormData["model"] == null)
            {
                //return "Bad Request";
                MessageModel modelDummy = new MessageModel();
                modelDummy.MessageID = "";
                modelDummy.Text = "Bad Request";
                return modelDummy;
            }

            var model = Newtonsoft.Json.JsonConvert.DeserializeObject<MessageModel>(result.FormData["model"]);
            //TODO: Do something with the json model which is currently a string

            string contenttype = string.Empty;
            //get the files
            foreach (var file in result.FileData)
            {
                switch (System.IO.Path.GetExtension(file.LocalFileName.Replace("\"", string.Empty)))
                {
                    case ".wav":
                        contenttype = "application/wav";
                        break;
                    case ".doc":
                        contenttype = "application/vnd.ms-word";
                        break;
                    case ".docx":
                        contenttype = "application/vnd.ms-word";
                        break;
                    case ".xls":
                        contenttype = "application/vnd.ms-excel";
                        break;
                    case ".xlsx":
                        contenttype = "application/vnd.ms-excel";
                        break;
                    case ".jpg":
                        contenttype = "image/jpg";
                        break;
                    case ".png":
                        contenttype = "image/png";
                        break;
                    case ".gif":
                        contenttype = "image/gif";
                        break;
                    case ".pdf":
                        contenttype = "application/pdf";
                        break;
                    case ".txt":
                        contenttype = "text/plain";
                        break;
                    case ".mp3":
                        contenttype = "audio/mp3";
                        break;
                }
                model.FileName = file.LocalFileName;

                var output = model.SaveVoiceMessage(contenttype, System.IO.File.ReadAllBytes(file.LocalFileName));
                if (output == "SUCCESS" && model.MessageStatus == "APPROVE")
                {
                    //string val = "bimalchawla89@gmail.com";
                    //if (model.To.ToUpper() == val.ToUpper())
                    //{
                    //GCMHelper.PushNotificationByUsername(model, model.To);
                    string registrationID = UserDetailsModel.GetRegistrationID(model.To, "1");
                    if (registrationID != string.Empty)
                    {
                        PushNotification obj = new PushNotification();
                        NotificaitonModel notificationmodel = new NotificaitonModel();
                        notificationmodel = NotificaitonModel.GetUserPushNotificationDetails(model.MessageReplyID);
                        if (notificationmodel != null)
                        {
                            obj.SendUserNotification(notificationmodel, registrationID);
                        }
                    }
                    //}
                }
                else if (output == "SUCCESS")
                {
                    //string val="Bhush.patilqa@gmail.com";
                    //if (model.To.ToUpper() == val.ToUpper())
                    //{
                    PushNotification obj = new PushNotification();
                    string registrationId = NotificaitonModel.GetAllModeators();
                    NotificaitonModel notificationmodel = new NotificaitonModel();
                    notificationmodel = NotificaitonModel.GeModelPushNotificationDetails(model.From, model.To);
                    if (registrationId != null)
                    {
                        notificationmodel.ayatollah = model.ayatollah;
                        notificationmodel.Subject = model.Subject;
                        notificationmodel.Text = model.Text;
                        notificationmodel.SendTo = model.To;
                        notificationmodel.SentFrom = model.From;
                        string[] regids = registrationId.Split(',');
                        foreach (string Regid in regids)
                        {
                            obj.SendModelNotification(notificationmodel, Regid);
                        }
                    }
                    //}
                }
                System.IO.File.Delete(model.FileName);
                var guid = Guid.NewGuid().ToString();
                // return model.FileID;
                if (output == "SUCCESS")
                {
                    MessageModel objModelResult = await model.GetJustSentMessageAsync(guid);
                    return objModelResult;
                }
                else
                {
                    model.MessageID = "";
                    return model;
                }
            }

            model.MessageID = "";
            model.Text = "Invalid File Type!";
            return model;

        }

        [Route("Message/UnansweredQueriesByUsername")]
        [HttpPost]
        [ApiAuthenticationFilter]
        //[AuthorizationRequired]
        public List<MessageModel> UnansweredQueriesByUsername(string scholarUsername, string userUsername)
        {
            return MessageModel.GetUnansweredQueriesByUsername(scholarUsername, userUsername);
        }

        [Route("Download")]
        [HttpGet]
        public HttpResponseMessage GetFile(string identifier)
        {
            HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
            FileHelper temp = MessageModel.GetFile(identifier);
            if (temp == null)
            {
                result = new HttpResponseMessage(HttpStatusCode.NotFound);
            }
            else
            {
                System.IO.Stream stream = new System.IO.MemoryStream(temp.Data);
                result.Content = new StreamContent(stream);
                result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
                result.Content.Headers.ContentDisposition.FileName = temp.FileName;
                return result;
            }

            //result.Content.Headers.ContentType =
            //    new MediaTypeHeaderValue("application/octet-stream");
            return result;

        }
        //Added By Abhi

        [Route("Message/SearchMessage")]
        [HttpGet]
        [ApiAuthenticationFilter]
        //[AuthorizationRequired]
        public List<MessageModel> GetMessageByKeyWord(string myUsername, string keyWord)
        {
            return MessageModel.GetMessageByKeyWord(myUsername, keyWord);
        }


        [Route("Message/GetLatestMessagesForUser")]
        [HttpGet]
        [ApiAuthenticationFilter]
        //[AuthorizationRequired]
        public List<MessageModel> GetLatestMessagesOfUser(string myUsername)
        {
            return MessageModel.GetLatestMessagesOfUser(myUsername);
        }

        [Route("Message/GetLatestSubjectMsg")]
        [HttpGet]
        [ApiAuthenticationFilter]
        //[AuthorizationRequired]
        public List<MessageModel> GetLatestSubjectMsg(string myUsername)
        {
            return MessageModel.GetLatestSubjectMsg(myUsername);
        }

        //[GET("Message/GetUserMsgForSubject")]
        [Route("Message/GetUserMsgForSubject")]
        [HttpGet]
        [ApiAuthenticationFilter]
        public List<MessageModel> GetUserMsgForSubject(string myUsername, string MsgID)
        {
            return MessageModel.GetUserMsgForSubject(myUsername, MsgID);
        }
        [Route("Message/Alter")]
        [HttpGet]
        [ApiAuthenticationFilter]
        public async Task Alter()
        {
            OleDBHelper oleDBHelper = new OleDBHelper();
            oleDBHelper.Alter();
        }
        //[GET("Message/GetUserMsgForSubject")]
        [Route("Message/GetMsgById")]
        [HttpGet]
        [ApiAuthenticationFilter]
        public async Task<MessageModel> GetMsgByIdAsync(string MsgID)
        {
            return await MessageModel.GetMsgByIdAsync(MsgID);
        }
        public async Task<UserDetailsModel> GetUserDetailsByUsernameAsync(string username)
        {
            var guid = Guid.NewGuid().ToString();
            FirebaseDBHelper firebaseDBHelper = new FirebaseDBHelper();
            await firebaseDBHelper.WriteLog(guid, "GetUserDetailsByUsernameAsync", "Entered");
            var data = (new OleDBHelper()).GetUserDetailsByUsername(username);
            var temp = new UserDetailsModel();
            temp.UserModel.UserType = data["userType"].ToString();
            temp.UserID = data["userID"].ToString();
            return temp;
        }
        [Route("Message/Test")]
        [HttpPost]
        [ApiAuthenticationFilter]
        //[AuthorizationRequired]
        public string Test(string MessageID, string UserID)
        {
            return MessageModel.MarkMultipleReadCommonMessages(MessageID, UserID);
        }
        [HttpPost]
        [Route("Message/DeleteMessage")]
        [ApiAuthenticationFilter]
        //[AuthorizationRequired]
        public async Task<string> DeleteMessageByIDAsync(string userName, string MsgID)//Emailid and messageID
        {

            var guid = Guid.NewGuid().ToString();
            FirebaseDBHelper firebaseDBHelper = new FirebaseDBHelper();
            var model = await GetMsgByIdAsync(MsgID);
            await firebaseDBHelper.WriteLog(guid, "Model", JsonConvert.SerializeObject(model));
            string result = "";
            result = await model.DeleteMessageAsync(model);
            if (model.MessageStatus == "PEND")
            {
                result = await model.DeleteMessageToModeratorReviewAsync(model);
            }
            int totalMessage = await CheckTotalMessageCount(model);
            if (totalMessage == 0)
            {
                result = await DeleteLatestSubject(model);
            }
            else
            {
                await UpdateLatestChatMessage(model);
            }

            return result;
        }


        private async Task<string> DeleteLatestSubject(MessageModel model)
        {
            FirebaseDBHelper firebaseDBHelper = new FirebaseDBHelper();
            string result = "";
            Dictionary<string, string> firebaseParameters = new Dictionary<string, string>();
            firebaseParameters.Add("FromUserType", model.FromUserType);
            firebaseParameters.Add("UserID", model.FromUserID);
            firebaseParameters.Add("ToID", model.ToUserID);
            firebaseParameters.Add("Subject", model.Subject);
            firebaseParameters.Add("Ayatollah", model.ayatollah);
            firebaseParameters.Add("MessageID", model.MessageID);

            var messageDetails = await firebaseDBHelper.GetLatestMessageDetails(firebaseParameters);
            if (messageDetails != null)
            {
                var messageDataJson = JsonConvert.SerializeObject(messageDetails);
                var latestMsgData = JsonConvert.DeserializeObject<MessageModel>(messageDataJson);
                if (latestMsgData.MessageID == model.MessageID)
                {
                    result = await model.DeleteLatestSubjectAsync(model);
                }
            }


            firebaseParameters["UserID"] = model.ToUserID;
            firebaseParameters["ToID"] = model.FromUserID;
            var messageDetails1 = await firebaseDBHelper.GetLatestMessageDetails(firebaseParameters);
            if (messageDetails1 != null)
            {
                var messageDataJson = JsonConvert.SerializeObject(messageDetails1);
                var latestMsgData = JsonConvert.DeserializeObject<MessageModel>(messageDataJson);
                if (latestMsgData.MessageID == model.MessageID)
                {
                    result = await model.DeleteLatestSubjectAsync(model);

                }
            }
            return result;
        }
        private async Task<string> UpdateLatestChatMessage(MessageModel model)
        {
            FirebaseDBHelper firebaseDBHelper = new FirebaseDBHelper();
            string result = "";
            Dictionary<string, string> firebaseParameters = new Dictionary<string, string>
            {
                { "FromUserType", model.FromUserType },
                { "UserID", model.FromUserID },
                { "ToID", model.ToUserID },
                { "Subject", model.Subject },
                { "Ayatollah", model.ayatollah },
                { "MessageID", model.MessageID },
                { "FromUserID", model.FromUserID },
                { "ToUserID", model.ToUserID }
            };
            var latestMessageDetails = await firebaseDBHelper.GetLatestMessageDetails(firebaseParameters);
            if (latestMessageDetails != null)
            {
                var latestMessageDataJson = JsonConvert.SerializeObject(latestMessageDetails);
                var latestMsgData = JsonConvert.DeserializeObject<MessageModel>(latestMessageDataJson);
                if (latestMsgData.MessageStatus == "APPROVE")
                {
                    if (latestMsgData.MessageID == model.MessageID)
                    {
                        var messageDetails = await firebaseDBHelper.GetLatestMessageToChatRoomAsync(firebaseParameters);
                        if (messageDetails != null)
                        {
                            var messageDataJson = JsonConvert.SerializeObject(messageDetails);
                            var latestChatMsgData = JsonConvert.DeserializeObject<MessageModel>(messageDataJson);
                            if (latestChatMsgData.MessageID != model.MessageID)
                            {
                                await model.UpdateLatestMessageAsync(latestChatMsgData);
                            }
                        }
                    }
                }
            }

            firebaseParameters["UserID"] = model.ToUserID;
            firebaseParameters["FromUserID"] = model.ToUserID;
            firebaseParameters["ToID"] = model.FromUserID;
            var latestMessageDetails1 = await firebaseDBHelper.GetLatestMessageDetails(firebaseParameters);
            if (latestMessageDetails1 != null)
            {
                var latestMessageDataJson = JsonConvert.SerializeObject(latestMessageDetails1);
                var latestMsgData = JsonConvert.DeserializeObject<MessageModel>(latestMessageDataJson);
                if (latestMsgData.MessageStatus == "APPROVE")
                {
                    if (latestMsgData.MessageID == model.MessageID)
                    {
                        var messageDetails = await firebaseDBHelper.GetLatestMessageToChatRoomAsync(firebaseParameters);
                        if (messageDetails != null)
                        {
                            var messageDataJson = JsonConvert.SerializeObject(messageDetails);
                            var latestChatMsgData = JsonConvert.DeserializeObject<MessageModel>(messageDataJson);
                            if (latestChatMsgData.MessageID != model.MessageID)
                            {
                                await model.UpdateLatestMessageAsync(latestChatMsgData);
                            }
                        }
                    }
                }
            }
            return result;
        }
        private async Task<int> CheckTotalMessageCount(MessageModel model)
        {
            FirebaseDBHelper firebaseDBHelper = new FirebaseDBHelper();
            Dictionary<string, string> firebaseParameters = new Dictionary<string, string>
            {
                { "FromUserType", model.FromUserType },
                { "UserID", model.FromUserID },
                { "ToID", model.ToUserID },
                { "Subject", model.Subject },
                { "Ayatollah", model.ayatollah },
                { "MessageID", model.MessageID },
                { "FromUserID", model.FromUserID },
                { "ToUserID", model.ToUserID }
            };

            int totalMessage = await firebaseDBHelper.CheckTotalMessageCount(firebaseParameters);
            return totalMessage;
        }

        private string getNewName(string fileName)
        {
            string extension = !string.IsNullOrWhiteSpace(fileName) ? System.IO.Path.GetExtension(fileName.Replace("\"", string.Empty)) : "";
            return Guid.NewGuid().ToString() + extension;
        }

        //public async Task<string> SendVoiceMessage(MessageModel message)
        //{
        //    var uploadPath = System.Web.Hosting.HostingEnvironment.MapPath("~/Uploads");
        //    var streamProvider = new CustomMultipartFormDataStreamProvider(uploadPath);
        //    //await Request.Content.ReadAsByteArrayAsync(streamProvider);

        //    await Request.Content.ReadAsMultipartAsync(streamProvider);

        //    //var task = Request.Content.ReadAsMultipartAsync(streamProvider).ContinueWith<IQueryable<HDFile>>(t =>
        //    //{
        //    //    if (t.IsFaulted || t.IsCanceled)
        //    //    {
        //    //        throw new HttpResponseException(HttpStatusCode.InternalServerError);
        //    //    }



        //    //});
        //}


        //[Route("Message/SendVoiceMessage")]
        //public async Task<HttpResponseMessage> SendVoiceMessage(MessageModel message)
        //{
        //    if (!Request.Content.IsMimeMultipartContent())
        //    {
        //        throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
        //    }
        //    MultipartMemoryStreamProvider provider = new MultipartMemoryStreamProvider();// (fileSaveLocation);

        //    byte[] x = await Request.Content.ReadAsByteArrayAsync();
        //    List<string> files = new List<string>();
        //    return Request.CreateResponse(HttpStatusCode.OK);
        //}

    }


    public static class ObjectExtensions
    {
        public static T ToObject<T>(this IDictionary<object, string> source)
            where T : class, new()
        {
            var someObject = new T();
            var someObjectType = someObject.GetType();

            foreach (var item in source)
            {
                someObjectType
                         .GetProperty(item.Key.ToString())
                         .SetValue(someObject, item.Value, null);
            }

            return someObject;
        }

        public static IDictionary<string, object> AsDictionary(this object source, BindingFlags bindingAttr = BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance)
        {
            return source.GetType().GetProperties(bindingAttr).ToDictionary
            (
                propInfo => propInfo.Name,
                propInfo => propInfo.GetValue(source, null)
            );

        }
    }

    public class CustomMultipartFormDataStreamProvider : MultipartFormDataStreamProvider
    {
        public CustomMultipartFormDataStreamProvider(string path) : base(path) { }

        public override string GetLocalFileName(HttpContentHeaders headers)
        {
            string extension = !string.IsNullOrWhiteSpace(headers.ContentDisposition.FileName) ? System.IO.Path.GetExtension(headers.ContentDisposition.FileName.Replace("\"", string.Empty)) : "";
            return Guid.NewGuid().ToString() + extension;
        }
    }


}
