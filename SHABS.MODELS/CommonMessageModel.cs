using ITEK.EMAILHELPER;
using SHABS.DB;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SHABS.MODELS
{
    public class CommonMessageModel : BaseModel
    {
        public string Text { get; set; }
        public string SentByUserName { get; set; }
        public string SentByUserID { get; set; }
        public string MsgID { get; set; }

        public List<CommonMessageModel> GetAllMessages()
        {
            List<CommonMessageModel> messages = new List<CommonMessageModel>();
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            DataTable dt1 = new DataTable();//RegisterUser
            dt1 = (new OleDBHelper()).GetData("GetCommonMessages", parameters);
            if (dt1.Rows.Count > 0)
            {
                for (int cntr = 0; cntr < dt1.Rows.Count; cntr++)
                {
                    CommonMessageModel temp = new CommonMessageModel();
                    temp.Text = dt1.Rows[cntr]["Text"].ToString();
                    temp.SentByUserName = dt1.Rows[cntr]["SentFrom"].ToString();
                    temp.SentByUserID = dt1.Rows[cntr]["createdBy"].ToString();
                    temp.CreatorUserId = dt1.Rows[cntr]["createdBy"].ToString();
                    temp.ModifierUserId = dt1.Rows[cntr]["modifiedBy"].ToString();
                    temp.CreatedDate = Convert.ToDateTime(dt1.Rows[cntr]["createdDate"]);
                    temp.ModifiedDate = Convert.ToDateTime(dt1.Rows[cntr]["modifiedDate"]);
                    temp.MsgID = dt1.Rows[cntr]["ID"].ToString();
                    messages.Add(temp);
                }
            }
            return messages;
        }

        public string Save()
        {
            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("@Text", Text);
            param.Add("@SentFrom", SentByUserName);
            param.Add("@Status", "ACTV");
            param.Add("@IsNotificationSent", "Y");
            param.Add("@createdBy", SentByUserID);
            param.Add("@modifiedBy", "0");
            int result = (new OleDBHelper()).InsertUpdateData("SendCommonMessage", param);
            Task.Run(() => SendNotification());
            if (result > 0)
            {
                return "SUCCESS";
            }
            else
            {
                return "Sorry Some Error Occured";
            }
        }

        public void SendNotification()
        {
            List<NotificaitonModel> test = new List<NotificaitonModel>();
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            DataTable dt = (new OleDBHelper()).GetData("GetOnlineUsers", parameters);
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                string input = Convert.ToString(dt.Rows[i]);
                if (string.IsNullOrEmpty(input) == false)
                {
                    NotificaitonModel temp = new NotificaitonModel();
                    temp.ToName = Convert.ToString(dt.Rows[i]["Name"]);
                    temp.RegistrationID = Convert.ToString(dt.Rows[i]["registrationID"].ToString());
                    temp.Text = Text;
                    if (string.IsNullOrEmpty(temp.RegistrationID) == false)
                    {
                        try
                        {
                            PushNotification2 obj = new PushNotification2();
                            obj.SendFCMNotification(temp);
                            System.Threading.Thread.Sleep(1000);
                            test.Add(temp);
                        }
                        catch (Exception)
                        {

                        }

                    }
                }

            }



        }

    }
}
