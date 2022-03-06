using SHABS.MODELS;
using System;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Text;
using System.Web.Configuration;

namespace ITEK.WEBAPI.Models
{
	public class PushNotification2
	{
		public PushNotification2()
		{
		}

		public void ProceedPush(string json)
		{
			try
			{
				WebRequest length = WebRequest.Create("https://fcm.googleapis.com/fcm/send");
				length.Method = "POST";
				length.Headers.Add(string.Format("Authorization: key={0}", WebConfigurationManager.AppSettings["APIKEY"]));
				length.ContentType = "application/json";
				byte[] bytes = Encoding.UTF8.GetBytes(json);
				length.ContentLength = (long)((int)bytes.Length);
				Stream requestStream = length.GetRequestStream();
				requestStream.Write(bytes, 0, (int)bytes.Length);
				requestStream.Close();
				WebResponse response = length.GetResponse();
				string statusDescription = ((HttpWebResponse)response).StatusDescription;
				requestStream = response.GetResponseStream();
				StreamReader streamReader = new StreamReader(requestStream);
				Console.WriteLine(streamReader.ReadToEnd());
				streamReader.Close();
				requestStream.Close();
				response.Close();
			}
			catch (Exception exception)
			{
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
            string[] title = new string[] { "{\"to\":\"", str, "\", \"priority\" : \"high\" ,\"notification\":{\"body\": \"", text, "\",\"title\" : \"", objMessage.Title, "\",\"content_available\": 1,\"sound\":\"default\"},\"data\":{\"Text\":\"", text, "\",\"SendTo\":\"", objMessage.SendTo, "\",\"SentFrom\":\"", objMessage.SentFrom, "\",\"Subject\":\"", objMessage.Subject, "\",\"ayatollah\":\"", objMessage.ayatollah, "\",\"UserType\":\"MODERATOR\",\"FromName\":\"", objMessage.FromName, "\",\"FromImageID\":\"", objMessage.FromImageID, "\",\"ToName\":\"", objMessage.ToName, "\",\"ToImageID\":\"", objMessage.ToImageID, "\",\"Title\":\"", objMessage.Title, "\",\"ContentType\":\"", objMessage.ContentType, "\"}}" };
			this.ProceedPush(string.Concat(title));
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
            string[] title = new string[] { "{\"to\":\"", str, "\", \"priority\" : \"high\" ,\"notification\":{\"body\": \"", text, "\",\"title\" : \"", objMessage.Title, "\",\"content_available\": 1,\"sound\":\"default\"},\"data\":{\"Text\":\"", objMessage.Text, "\",\"SendTo\":\"", objMessage.SendTo, "\",\"SentFrom\":\"", objMessage.SentFrom, "\",\"Subject\":\"", objMessage.Subject, "\",\"ayatollah\":\"", objMessage.ayatollah, "\",\"UserType\":\"", objMessage.UserType, "\",\"FromName\":\"", objMessage.FromName, "\",\"FromImageID\":\"", objMessage.FromImageID, "\",\"ToName\":\"", objMessage.ToName, "\",\"ToImageID\":\"", objMessage.ToImageID, "\",\"Title\":\"", objMessage.Title, "\",\"ReplyToID\":\"", objMessage.ReplyToID, "\",\"Status\":\"", objMessage.Status, "\",\"IsRead\":\"", objMessage.IsRead, "\",\"createdBy\":\"", objMessage.createdBy, "\",\"ModifiedBy\":\"", objMessage.ModifiedBy, "\",\"createdDate\":\"", null, null, null, null, null, null, null, null, null, null };
			DateTime dateTime = Convert.ToDateTime(objMessage.createdDate);
			title[39] = dateTime.ToString();
			title[40] = "\",\"ModifiedDate\":\"";
			DateTime dateTime1 = Convert.ToDateTime(objMessage.ModifiedDate);
			title[41] = dateTime1.ToString();
			title[42] = "\",\"IsReplied\":\"";
			title[43] = objMessage.IsReplied;
			title[44] = "\",\"IsVoice\":\"";
			title[45] = objMessage.IsVoice;
            title[46] = "\",\"ContentType\":\"";
            title[47] = objMessage.ContentType;
			title[48] = "\"}}";
			this.ProceedPush(string.Concat(title));
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
                string text = string.Format("Your question '{0}' is redirected to '{1}'",objMessage.Subject,objMessage.ToName);
                objMessage.isRedirect = "Y";
                //if (objMessage.IsVoice == "Y")
                //{
                //    text = "Audio Message Received.Please download to listen.";
                //}
                //else if (string.IsNullOrEmpty(objMessage.ContentType) == false)
                //{
                //    text = "Document Received. Please download to read.";
                //}
                string[] title = new string[] { "{\"to\":\"", str, "\", \"priority\" : \"high\" ,\"notification\":{\"body\": \"", text, "\",\"title\" : \"", objMessage.Title, "\",\"content_available\": 1,\"sound\":\"default\"},\"data\":{\"Text\":\"", objMessage.Text, "\",\"SendTo\":\"", objMessage.SendTo, "\",\"SentFrom\":\"", objMessage.SentFrom, "\",\"Subject\":\"", objMessage.Subject, "\",\"ayatollah\":\"", objMessage.ayatollah, "\",\"UserType\":\"", objMessage.UserType, "\",\"FromName\":\"", objMessage.FromName, "\",\"FromImageID\":\"", objMessage.FromImageID, "\",\"ToName\":\"", objMessage.ToName, "\",\"ToImageID\":\"", objMessage.ToImageID, "\",\"Title\":\"", objMessage.Title, "\",\"ReplyToID\":\"", objMessage.ReplyToID, "\",\"Status\":\"", objMessage.Status, "\",\"IsRead\":\"", objMessage.IsRead, "\",\"createdBy\":\"", objMessage.createdBy, "\",\"ModifiedBy\":\"", objMessage.ModifiedBy, "\",\"createdDate\":\"", null, null, null, null, null, null, null, null, null, null, null, null };
				DateTime dateTime = Convert.ToDateTime(objMessage.createdDate);
				title[39] = dateTime.ToString();
				title[40] = "\",\"ModifiedDate\":\"";
				DateTime dateTime1 = Convert.ToDateTime(objMessage.ModifiedDate);
				title[41] = dateTime1.ToString();
				title[42] = "\",\"IsReplied\":\"";
				title[43] = objMessage.IsReplied;
				title[44] = "\",\"IsVoice\":\"";
				title[45] = objMessage.IsVoice;
                title[46] = "\",\"isRedirect\":\"";
                title[47] = objMessage.isRedirect;
                title[48] = "\",\"ContentType\":\"";
                title[49] = objMessage.ContentType;
				title[50] = "\"}}";
				this.ProceedPush(string.Concat(title));
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
            string[] title = new string[] { "{\"to\":\"", str, "\", \"priority\" : \"high\" ,\"notification\":{\"body\": \"", text, "\",\"title\" : \"", objMessage.Title, "\",\"content_available\": 1,\"sound\":\"default\"},\"data\":{\"Text\":\"", objMessage.Text, "\",\"SendTo\":\"", objMessage.SendTo, "\",\"SentFrom\":\"", objMessage.SentFrom, "\",\"Subject\":\"", objMessage.Subject, "\",\"ayatollah\":\"", objMessage.ayatollah, "\",\"UserType\":\"", objMessage.UserType, "\",\"FromName\":\"", objMessage.FromName, "\",\"FromImageID\":\"", objMessage.FromImageID, "\",\"ToName\":\"", objMessage.ToName, "\",\"ToImageID\":\"", objMessage.ToImageID, "\",\"Title\":\"", objMessage.Title, "\",\"ReplyToID\":\"", objMessage.ReplyToID, "\",\"Status\":\"", objMessage.Status, "\",\"IsRead\":\"", objMessage.IsRead, "\",\"createdBy\":\"", objMessage.createdBy, "\",\"ModifiedBy\":\"", objMessage.ModifiedBy, "\",\"createdDate\":\"", null, null, null, null, null, null, null, null, null, null };
			DateTime dateTime = Convert.ToDateTime(objMessage.createdDate);
			title[39] = dateTime.ToString();
			title[40] = "\",\"ModifiedDate\":\"";
			DateTime dateTime1 = Convert.ToDateTime(objMessage.ModifiedDate);
			title[41] = dateTime1.ToString();
			title[42] = "\",\"IsReplied\":\"";
			title[43] = objMessage.IsReplied;
			title[44] = "\",\"IsVoice\":\"";
			title[45] = objMessage.IsVoice;
            title[46] = "\",\"ContentType\":\"";
            title[47] = objMessage.ContentType;
            title[48] = "\"}}";
			this.ProceedPush(string.Concat(title));
		}

		public void SendUserNotification(NotificaitonModel objMessage, string RegistrationId)
		{
			string str = "";
			str = (WebConfigurationManager.AppSettings["Test"] != "1" ? RegistrationId : WebConfigurationManager.AppSettings["BimalID"]);
            string messagetext = objMessage.Text;
            if (objMessage.IsVoice == "Y")
            {
                messagetext = "Audio Message Received.Please download to listen.";
            }
            else if (string.IsNullOrEmpty(objMessage.ContentType) == false)
            {
                messagetext = "Attachment Received. Please download to view.";
            }
			objMessage.Title = string.Concat(objMessage.FromName, " replied to :", objMessage.Subject);
            string[] text = new string[] { "{\"to\":\"", str, "\", \"priority\" : \"high\" ,\"notification\":{\"body\": \"", messagetext, "\",\"title\" : \"", objMessage.Title, "\",\"content_available\": 1,\"sound\":\"default\"},\"data\":{ \"Text\":\"", objMessage.Text, "\",\"SendTo\":\"", objMessage.SendTo, "\",\"SentFrom\":\"", objMessage.SentFrom, "\",\"Subject\":\"", objMessage.Subject, "\",\"ayatollah\":\"", objMessage.ayatollah, "\",\"UserType\":\"", objMessage.UserType, "\",\"FromName\":\"", objMessage.FromName, "\",\"FromImageID\":\"", objMessage.FromImageID, "\",\"ToName\":\"", objMessage.ToName, "\",\"ToImageID\":\"", objMessage.ToImageID, "\",\"Title\":\"", objMessage.Title, "\",\"ReplyToID\":\"", objMessage.ReplyToID, "\",\"Status\":\"", objMessage.Status, "\",\"IsRead\":\"", objMessage.IsRead, "\",\"createdBy\":\"", objMessage.createdBy, "\",\"ModifiedBy\":\"", objMessage.ModifiedBy, "\",\"createdDate\":\"", null, null, null, null, null, null, null, null, null, null };
			DateTime dateTime = Convert.ToDateTime(objMessage.createdDate);
			text[39] = dateTime.ToString();
			text[40] = "\",\"ModifiedDate\":\"";
			DateTime dateTime1 = Convert.ToDateTime(objMessage.ModifiedDate);
			text[41] = dateTime1.ToString();
			text[42] = "\",\"IsReplied\":\"";
			text[43] = objMessage.IsReplied;
			text[44] = "\",\"IsVoice\":\"";
			text[45] = objMessage.IsVoice;
            text[46] = "\",\"ContentType\":\"";
            text[47] = objMessage.ContentType;
            text[48] = "\"}}";
			this.ProceedPush(string.Concat(text));
		}


        public void SendFCMNotification(NotificaitonModel objMessage)
        {
            string str = "";
            str = (System.Configuration.ConfigurationManager.AppSettings["Test"] != "1" ? objMessage.RegistrationID : System.Configuration.ConfigurationManager.AppSettings["BimalID"]);
            objMessage.Title = string.Format("Hi {0}, You have unread messages", objMessage.ToName);
            string text = string.Format("Hi {0}, You have unread messages. Please login to the app to read them.", objMessage.ToName);

            string message = "{\"to\":\"" + str + "\" ,\"priority\" : \"high\" ,\"notification\":{\"body\": \"" + text + "\" , \"title\" : \"" + objMessage.Title + "\",\"content_available\": 1,\"sound\":\"default\"},\"data\":{ \"Text\":\"" + text + "\" } }";
            this.ProceedPush(message);
        }
	}
}