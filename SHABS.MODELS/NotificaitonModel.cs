using SHABS.DB;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SHABS.MODELS
{
   public class NotificaitonModel
    {
        public string ID { get; set; }
        public string Text { get; set; }
        public string SendTo { get; set; }
        public string SentFrom { get; set; }
        public string ReplyToID { get; set; }
        public string Status { get; set; }
        public string IsRead { get; set; }
        public string createdBy { get; set; }
        public string ModifiedBy { get; set; }
        public string createdDate { get; set; }
        public string ModifiedDate { get; set; }
        public string IsReplied { get; set; }
        public string IsVoice { get; set; }
        public string Subject { get; set; }
        public string ayatollah { get; set; }
        public string UserType { get; set; }
        public string FromUserType { get; set; }
        public string FromName { get; set; }
        public string FromImageID { get; set; }
        public string ToName { get; set; }
        public string ToImageID { get; set; }
        public string Title { get; set; }
        public string ToUserID { get; set; }
        public string FromUserID { get; set; }
        public string isRedirect { get; set; }
        public string RegistrationID { get; set; }
        public string ContentType { get; set; }

        public string FileSize { get; set; }
        public string FileName { get; set; }
        public string FileThumbNail { get; set; }
        public string FileContextText { get; set; }

        public static NotificaitonModel FillModel(DataRow dr)
        {
            NotificaitonModel x = new NotificaitonModel();
            x.ID = dr["ID"].ToString();
            x.Text = dr["Text"].ToString();
            x.SendTo = dr["SendTo"].ToString();
            x.SentFrom = dr["SentFrom"].ToString();
            x.ReplyToID = dr["ReplyToID"].ToString();
            x.Status = dr["Status"].ToString();
            x.IsRead = dr["IsRead"].ToString();
            x.createdBy = dr["createdBy"].ToString();
            x.ModifiedBy = dr["ModifiedBy"].ToString();
            x.createdDate = dr["createdDate"].ToString();
            x.ModifiedDate = dr["ModifiedDate"].ToString();
            x.IsReplied = dr["IsReplied"].ToString();
            x.IsVoice = dr["IsVoice"].ToString();
            x.Subject = dr["Subject"].ToString();
            x.ayatollah = dr["ayatollah"].ToString();
            x.UserType = dr["UserType"].ToString();
            x.FromUserType = dr["UserType"].ToString() == "ALIM" ? "USER" : "ALIM";
            x.FromName = dr["FromName"].ToString();
            x.FromImageID = dr["FromImageID"].ToString();
            x.ToName = dr["ToName"].ToString();
            x.ToImageID = dr["ToImageID"].ToString();
            try
            {
                x.ContentType = dr["ContentType"].ToString();
                x.FileName = dr["FileTitle"].ToString();
                x.FileSize = dr["FileSize"].ToString();
                x.FileThumbNail = dr["FileThumbnailID"].ToString();
                x.FileContextText = dr["FileContextText"].ToString();

                String user_from = x.SentFrom;
                String user_to = x.SendTo;
                UserDetailsModel fromModel = UserDetailsModel.GetUserDetailsByUsernameAsync(user_from);
                UserDetailsModel toModel = UserDetailsModel.GetUserDetailsByUsernameAsync(user_to);
                
                x.FromUserID = fromModel.UserModel.UserId;
                x.ToUserID = toModel.UserModel.UserId;
                x.FromUserType = fromModel.UserModel.UserType;
            }
            catch (Exception)
            {
                x.ContentType = string.Empty;
            }
            return x;
        }

        public static NotificaitonModel GetUserPushNotificationDetails(string Id)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            NotificaitonModel temp = new NotificaitonModel();
            parameters.Add("@id", Id.Trim());

            DataTable dt1 = new DataTable();//RegisterUser
            dt1 = (new OleDBHelper()).GetData("GetUserPushNotificationDetails", parameters);
            if (dt1.Rows.Count > 0)
            {
                for (int cntr = 0; cntr < dt1.Rows.Count; cntr++)
                {
                    
                    temp = FillModel(dt1.Rows[cntr]);
                }
            }
            return temp;
        }

        public static string GetAllModeators()
        {
            string temp = ""; ;     

            DataTable dt1 = new DataTable();//RegisterUser
            dt1 = (new OleDBHelper()).GetData("GetAllModeators");
            if (dt1.Rows.Count > 0)
            {
                for (int cntr = 0; cntr < dt1.Rows.Count; cntr++)
                {

                    if (cntr == 0)
                    {
                        temp = ""+dt1.Rows[cntr]["RegistrationId"].ToString().Trim()+"";
                    }
                    else
                    {
                        temp += "," + dt1.Rows[cntr]["RegistrationId"].ToString().Trim() + "";
                    }
                }
                //temp += "]";
            }   
            return temp;
        }

        public static NotificaitonModel GetSchollerPushNotificationDetails(string Id)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            NotificaitonModel temp = new NotificaitonModel();
            parameters.Add("@id", Id.Trim());

            DataTable dt1 = new DataTable();//RegisterUser
            dt1 = (new OleDBHelper()).GetData("GetSchollerPushNotificationDetails", parameters);
            if (dt1.Rows.Count > 0)
            {
                for (int cntr = 0; cntr < dt1.Rows.Count; cntr++)
                {
                    
                    temp = FillModel(dt1.Rows[cntr]);
                }
            }
            return temp;
        }

        public static List<NotificaitonModel> GetUserNotification(string MailId)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            List<NotificaitonModel> temp = new List<NotificaitonModel>();
            parameters.Add("@MailId", MailId);

            DataTable dt1 = new DataTable();//RegisterUser
            dt1 = (new OleDBHelper()).GetData("GetUserNotification", parameters);
            if (dt1.Rows.Count > 0)
            {
                for (int cntr = 0; cntr < dt1.Rows.Count; cntr++)
                {
                    NotificaitonModel x = new NotificaitonModel();
                    x = FillModel(dt1.Rows[cntr]);
                    temp.Add(x);
                }
            }
            return temp;
        }


        public static List<string> GetUserNotificationCount(string MailId)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            List<string> temp = new List<string>();
            parameters.Add("@MailId", MailId);
            string resStr = "";
            DataTable dt1 = new DataTable();//RegisterUser
            dt1 = (new OleDBHelper()).GetData("GetUserNotificationCount", parameters);
            if (dt1.Rows.Count > 0)
            {
                temp.Add("ReplyCount:" + dt1.Rows[0]["ReplyCount"].ToString());
                temp.Add("CommonMsgCount:" + dt1.Rows[0]["CommonMsgCount"].ToString());
                temp.Add("TotalCount:" + dt1.Rows[0]["TotalCount"].ToString());
            }
            return temp;
        }

        public static List<NotificaitonModel> GetScollerNotification(string MailId)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            List<NotificaitonModel> temp = new List<NotificaitonModel>();
            parameters.Add("@MailId", MailId);

            DataTable dt1 = new DataTable();//RegisterUser
            dt1 = (new OleDBHelper()).GetData("GetScollerNotification", parameters);
            if (dt1.Rows.Count > 0)
            {
                for (int cntr = 0; cntr < dt1.Rows.Count; cntr++)
                {
                    NotificaitonModel x = new NotificaitonModel();
                    x = FillModel(dt1.Rows[cntr]);
                    temp.Add(x);
                }
            }
            return temp;
        }

        public static NotificaitonModel GeModelPushNotificationDetails(string FMailId, string ToMailID)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            List<NotificaitonModel> temp = new List<NotificaitonModel>();
            parameters.Add("@FMailId", FMailId);
            parameters.Add("@TMailId", ToMailID);
            NotificaitonModel x = new NotificaitonModel();
            DataTable dt1 = new DataTable();//RegisterUser
            dt1 = (new OleDBHelper()).GetData("GeModelPushNotificationDetails", parameters);
            if (dt1.Rows.Count > 0)
            {
                for (int cntr = 0; cntr < dt1.Rows.Count; cntr++)
                {
                    
                    x.ID = null;
                    x.Text = null;
                    x.SendTo = null;
                    x.SentFrom = null;
                    x.ReplyToID = null;
                    x.Status = null;
                    x.IsRead = null;
                    x.createdBy = null;
                    x.ModifiedBy = null;
                    x.createdDate = null;
                    x.ModifiedDate = null;
                    x.IsReplied = null;
                    x.IsVoice = null;
                    x.Subject = null;
                    x.ayatollah = null;
                    x.UserType = dt1.Rows[cntr]["UserType"].ToString();
                    x.FromName = dt1.Rows[cntr]["FromName"].ToString();
                    x.FromImageID = dt1.Rows[cntr]["FromImageID"].ToString();
                    x.ToName = dt1.Rows[cntr]["ToName"].ToString();
                    x.ToImageID = dt1.Rows[cntr]["ToImageID"].ToString();
                    try
                    {
                        x.ContentType = dt1.Rows[cntr]["ContentType"].ToString();
                        x.FileName = dt1.Rows[cntr]["FileTitle"].ToString();
                        x.FileSize = dt1.Rows[cntr]["FileSize"].ToString();
                        x.FileThumbNail = dt1.Rows[cntr]["FileThumbnailID"].ToString();
                       // ,m.ContentType,m.FileTitle,m.FileSize,m.FileThumbnailID
                    }
                    catch (Exception)
                    {
                        x.ContentType = string.Empty;
                    }
                }
            }
            return x;
        }

        public static List<NotificaitonModel> GetModiatorNotification()
        {
            List<NotificaitonModel> temp = new List<NotificaitonModel>();
            DataTable dt1 = new DataTable();//RegisterUser
            dt1 = (new OleDBHelper()).GetData("GetModiatorNotification");
            if (dt1.Rows.Count > 0)
            {
                for (int cntr = 0; cntr < dt1.Rows.Count; cntr++)
                {
                    NotificaitonModel x = new NotificaitonModel();
                    x = FillModel(dt1.Rows[cntr]);
                    temp.Add(x);
                }
            }
            return temp;
        }
    }
}
