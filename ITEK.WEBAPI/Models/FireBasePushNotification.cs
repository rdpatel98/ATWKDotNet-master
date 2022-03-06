using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;
using ITEK.EMAILHELPER;
using SHABS.MODELS;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Hosting;

namespace ITEK.WEBAPI.Models
{
    public class PushNotification
    {
        FirebaseAdmin.FirebaseApp firebaseApp;

        public PushNotification()
        {
           
            FirebaseAdmin.AppOptions appOptions = new FirebaseAdmin.AppOptions();
            appOptions.Credential = GoogleCredential.FromFile(HostingEnvironment.MapPath(@"~/ask-those-who-know-155117-firebase-adminsdk-w3k1j-1af8495615.json"));
            //firebaseApp = FirebaseMessaging.DefaultInstance ?? FirebaseAdmin.FirebaseApp.Create(appOptions);
            if(FirebaseMessaging.DefaultInstance == null)
            {
                firebaseApp = FirebaseAdmin.FirebaseApp.Create(appOptions);
            }
            
        }

        public void SendModelNotification(NotificaitonModel objMessage, string RegistrationId)
        {
            string str = "";
            str = (WebConfigurationManager.AppSettings["Test"] != "1" ? RegistrationId : WebConfigurationManager.AppSettings["BimalID"]);
            objMessage.Title = string.Concat(objMessage.FromName, " sent query ", objMessage.ToName);
            string text = objMessage.Text;
            if (objMessage.IsVoice == "Y")
            {
                text = "Audio Message Received.Please download to listen.";
            }
            else if (string.IsNullOrEmpty(objMessage.ContentType) == false)
            {
                text = "Attachment Received. Please download to view.";
            }
            Dictionary<string, string> data = new Dictionary<string, string>();
            //text = text + objMessage.ID;
            data.Add("Text",text);
            data.Add("Title", objMessage.Title);
            data.Add("ContentType", objMessage.ContentType);
            data.Add("SendTo",objMessage.SendTo);
            data.Add("SentFrom",objMessage.SentFrom);
            data.Add("Subject", objMessage.Subject);
            data.Add("ayatollah", objMessage.ayatollah);
            data.Add("UserType", "MODERATOR");
            data.Add("FromName", objMessage.FromName);
            data.Add("FromImageID", objMessage.FromImageID);
            data.Add("ToName",objMessage.ToName);
            data.Add("ToImageID",objMessage.ToImageID);
            data.Add("Id", objMessage.ID);
            SendNotification(text, objMessage.Title, str.Split(',').ToList(), data);
            //not required for moderator
            //System.Threading.Tasks.Task.Run(() =>
            //{
            //    SendEmailNotifciation(text, objMessage.SendTo, "New Message Received on ATWK");
            //});
        }


        public void SendNotification(NotificaitonModel objMessage, string RegistrationId)
        {
            string str = "";
            str = (WebConfigurationManager.AppSettings["Test"] != "1" ? RegistrationId : WebConfigurationManager.AppSettings["BimalID"]);
            string text = objMessage.Text;
            if (objMessage.IsVoice == "Y")
            {
                text = "Audio Message Received.Please download to listen.";
            }
            else if (string.IsNullOrEmpty(objMessage.ContentType) == false)
            {
                text = "Attachment Received. Please download to view.";
            }
            objMessage.Title = objMessage.Subject;
            Dictionary<string, string> data = new Dictionary<string, string>();
            //text = text + objMessage.ID;
            data.Add("Text", text);
            data.Add("Title", objMessage.Title);
            data.Add("ContentType", objMessage.ContentType);
            data.Add("SendTo", objMessage.SendTo);
            data.Add("SentFrom", objMessage.SentFrom);
            data.Add("Subject", objMessage.Subject);
            data.Add("ayatollah", objMessage.ayatollah);
            data.Add("UserType", "");
            data.Add("FromName", objMessage.FromName);
            data.Add("FromImageID", objMessage.FromImageID);
            data.Add("ToName", objMessage.ToName);
            data.Add("ToImageID", objMessage.ToImageID);
            data.Add("ReplyToID", objMessage.ReplyToID);
            data.Add("Status", objMessage.Status);
            data.Add("IsRead", objMessage.IsRead);
            data.Add("createdBy", objMessage.createdBy);
            data.Add("ModifiedBy", objMessage.ModifiedBy);
            data.Add("createdDate", objMessage.createdDate);
            data.Add("ModifiedDate", objMessage.ModifiedDate);
            data.Add("IsReplied", objMessage.IsReplied);
            data.Add("IsVoice", objMessage.IsVoice);
            data.Add("Id", objMessage.ID);

            //data.Add("createdDate", objMessage.createdDate);
            SendNotification(text, objMessage.Title, str.Split(',').ToList(), data);
            System.Threading.Tasks.Task.Run(() =>
            {
                SendEmailNotifciation(text, objMessage.SendTo, "New Message Received on ATWK");
            });
        }


        public void SendRedirectNotification(NotificaitonModel objMessage, string RegistrationId)
        {
            try
            {
                string str = "";
                str = (WebConfigurationManager.AppSettings["Test"] != "1" ? RegistrationId : WebConfigurationManager.AppSettings["BimalID"]);
                //objMessage.Title = string.Format("Your message is now redirected to {0}", objMessage.ToName);
                objMessage.Title = "Redirected";
                objMessage.UserType = "USER";
                //string text = objMessage.Text;
                string text = string.Format("Your question '{0}' is redirected to '{1}'", objMessage.Subject, objMessage.ToName);
                objMessage.isRedirect = "Y";
                Dictionary<string, string> data = new Dictionary<string, string>();
                //text = text + objMessage.ID;
                data.Add("Text", text);
                data.Add("Title", objMessage.Title);
                data.Add("ContentType", objMessage.ContentType);
                data.Add("SendTo", objMessage.SendTo);
                data.Add("SentFrom", objMessage.SentFrom);
                data.Add("Subject", objMessage.Subject);
                data.Add("ayatollah", objMessage.ayatollah);
                data.Add("UserType", objMessage.UserType);
                data.Add("FromName", objMessage.FromName);
                data.Add("FromImageID", objMessage.FromImageID);
                data.Add("ToName", objMessage.ToName);
                data.Add("ToImageID", objMessage.ToImageID);
                data.Add("ReplyToID", objMessage.ReplyToID);
                data.Add("Status", objMessage.Status);
                data.Add("IsRead", objMessage.IsRead);
                data.Add("createdBy", objMessage.createdBy);
                data.Add("ModifiedBy", objMessage.ModifiedBy);
                data.Add("createdDate", objMessage.createdDate);
                data.Add("ModifiedDate", objMessage.ModifiedDate);
                data.Add("IsReplied", objMessage.IsReplied);
                data.Add("IsVoice", objMessage.IsVoice);
                data.Add("Id", objMessage.ID);
                //data.Add("createdDate", objMessage.createdDate);
                SendNotification(text, objMessage.Title, str.Split(',').ToList(), data);
                System.Threading.Tasks.Task.Run(() =>
                {
                    SendEmailNotifciation(text, objMessage.SendTo, "New Message Received on ATWK");
                });
            }
            catch (Exception exception)
            {
            }
        }

        public void SendScholerNotification(NotificaitonModel objMessage, string RegistrationId)
        {
            string str = "";
            str = (WebConfigurationManager.AppSettings["Test"] != "1" ? RegistrationId : WebConfigurationManager.AppSettings["BimalID"]);
            objMessage.Title = string.Concat(objMessage.FromName, " sent query : ", objMessage.Subject);
            string text = objMessage.Text;
            if (objMessage.IsVoice == "Y")
            {
                text = "Audio Message Received.Please download to listen.";
            }
            else if (string.IsNullOrEmpty(objMessage.ContentType) == false)
            {
                text = "Attachment Received. Please download to view.";
            }
            Dictionary<string, string> data = new Dictionary<string, string>();
            //text = text + objMessage.ID;
            data.Add("Text", text);
            data.Add("Title", objMessage.Title);
            data.Add("ContentType", objMessage.ContentType);
            data.Add("SendTo", objMessage.SendTo);
            data.Add("SentFrom", objMessage.SentFrom);
            data.Add("Subject", objMessage.Subject);
            data.Add("ayatollah", objMessage.ayatollah);
            data.Add("UserType", objMessage.UserType);
            data.Add("FromName", objMessage.FromName);
            data.Add("FromImageID", objMessage.FromImageID);
            data.Add("ToName", objMessage.ToName);
            data.Add("ToImageID", objMessage.ToImageID);
            data.Add("ReplyToID", objMessage.ReplyToID);
            data.Add("Status", objMessage.Status);
            data.Add("IsRead", objMessage.IsRead);
            data.Add("createdBy", objMessage.createdBy);
            data.Add("ModifiedBy", objMessage.ModifiedBy);
            data.Add("createdDate", objMessage.createdDate);
            data.Add("ModifiedDate", objMessage.ModifiedDate);
            data.Add("IsReplied", objMessage.IsReplied);
            data.Add("IsVoice", objMessage.IsVoice);
            data.Add("Id", objMessage.ID);
            //data.Add("createdDate", objMessage.createdDate);
            SendNotification(text, objMessage.Title, str.Split(',').ToList(), data);
            System.Threading.Tasks.Task.Run(() =>
            {
                SendEmailNotifciation(text, objMessage.SendTo, "New Message Received on ATWK");
            });
        }

        public void SendUserNotification(NotificaitonModel objMessage, string RegistrationId)
        {
            string str = "";
            str = (WebConfigurationManager.AppSettings["Test"] != "1" ? RegistrationId : WebConfigurationManager.AppSettings["BimalID"]);
            objMessage.Title = string.Concat(objMessage.FromName, " replied to : ", objMessage.Subject);
            string messagetext = objMessage.Text;
            if (objMessage.IsVoice == "Y")
            {
                messagetext = "Audio Message Received.Please download to listen.";
            }
            else if (string.IsNullOrEmpty(objMessage.ContentType) == false)
            {
                messagetext = "Attachment Received. Please download to view.";
            }
            Dictionary<string, string> data = new Dictionary<string, string>();
            //messagetext = messagetext + objMessage.ID;
            data.Add("Text", messagetext);
            data.Add("Title", objMessage.Title);
            data.Add("ContentType", objMessage.ContentType);
            data.Add("SendTo", objMessage.SendTo);
            data.Add("SentFrom", objMessage.SentFrom);
            data.Add("Subject", objMessage.Subject);
            data.Add("ayatollah", objMessage.ayatollah);
            data.Add("UserType", objMessage.UserType);
            data.Add("FromName", objMessage.FromName);
            data.Add("FromImageID", objMessage.FromImageID);
            data.Add("ToName", objMessage.ToName);
            data.Add("ToImageID", objMessage.ToImageID);
            data.Add("ReplyToID", objMessage.ReplyToID);
            data.Add("Status", objMessage.Status);
            data.Add("IsRead", objMessage.IsRead);
            data.Add("createdBy", objMessage.createdBy);
            data.Add("ModifiedBy", objMessage.ModifiedBy);
            data.Add("createdDate", objMessage.createdDate);
            data.Add("ModifiedDate", objMessage.ModifiedDate);
            data.Add("IsReplied", objMessage.IsReplied);
            data.Add("IsVoice", objMessage.IsVoice);
            data.Add("Id", objMessage.ID);
            //data.Add("createdDate", objMessage.createdDate);
            SendNotification(messagetext, objMessage.Title, str.Split(',').ToList(), data);
            System.Threading.Tasks.Task.Run(() =>
            {
                SendEmailNotifciation(messagetext, objMessage.SendTo, "New Message Received on ATWK");
            });
        }

        public void SendEmailNotifciation(string messageText,string emailID,string subject)
        {
            try
            {
                string SmtpServer = ConfigurationManager.AppSettings["SMTPServer"];
                int SmtpPort = Convert.ToInt32(ConfigurationManager.AppSettings["SMTPPort"]);
                string EmailUserName = ConfigurationManager.AppSettings["EmailUserName"];
                string EmailPassword = ConfigurationManager.AppSettings["EmailPassword"];
                bool IsSSL = ConfigurationManager.AppSettings["EnableSSL"] == "1";
                string fromEmailID = ConfigurationManager.AppSettings["EmailID"];

                EmailMessage emailClient = new EmailMessage();
                emailClient.Message = messageText;
                //emailClient.Subject = "MyApp: ActivationLink";
                emailClient.Subject = subject;
                emailClient.To = new List<string> { emailID };
                emailClient.SendEmail(SmtpServer, SmtpPort, fromEmailID, IsSSL, EmailUserName, EmailPassword);
            }
            catch (Exception)
            {
            }
        }

        public void SendNotification(string body, string title, string registrationToken, Dictionary<string, string> data = null)
        {
            var message = new FirebaseAdmin.Messaging.Message()
            {
                Notification = new Notification()
                {
                    Body = body,
                    Title = title
                },
                Token = registrationToken
                
            };

            if (data != null)
            {
                message.Data = data;
            }

            AndroidConfig androidConfig = new AndroidConfig();
            ApnsConfig apnsConfig = new ApnsConfig();
            try
            {
                var response = FirebaseMessaging.DefaultInstance.SendAsync(message).Result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void SendNotification(string body, string title, List<string> registrationTokens, Dictionary<string,string> data= null)
        {
            var message = new FirebaseAdmin.Messaging.MulticastMessage()
            {
                Notification = new Notification()
                {
                    Body = body,
                    Title = title
                },
                Tokens = registrationTokens

            };

            if(data!=null)
            {
                message.Data = data;
            }
            try
            {
                var response = FirebaseMessaging.DefaultInstance.SendMulticastAsync(message).Result;
            }
            catch (Exception)
            {

            }
          
        }



    }
}