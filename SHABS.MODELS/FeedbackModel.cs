using ITEK.EMAILHELPER;
using SHABS.DB;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SHABS.MODELS
{
    public class FeedbackModel:BaseModel
    {
        public string UserID { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }


        public string Save()
        {
            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("@userid", UserID);
            param.Add("@subject", Subject);
            param.Add("@message", Message);
            int result = (new OleDBHelper()).InsertUpdateData("AddFeedback", param);
            Task.Run(() => SendMailAsync());
            if (result > 0)
            {
                return "SUCCESS";
            }
            else
            {
                return "Sorry Some Error Occured";
            }
        }

        public async Task SendMailAsync()
        {
            try
            {
                var user = await UserDetailsModel.GetUserDetailsAsync(UserID);
                string SmtpServer = ConfigurationManager.AppSettings["SMTPServer"];
                int SmtpPort = Convert.ToInt32(ConfigurationManager.AppSettings["SMTPPort"]);
                string EmailUserName = ConfigurationManager.AppSettings["EmailUserName"];
                string EmailPassword = ConfigurationManager.AppSettings["EmailPassword"];
                bool IsSSL = ConfigurationManager.AppSettings["EnableSSL"] == "1";
                string fromEmailID = ConfigurationManager.AppSettings["EmailID"];
                string receiver = ConfigurationManager.AppSettings["receiver"];
                var temp = receiver.Split(';').ToList();
                EmailMessage emailClient = new EmailMessage();
                emailClient.ReplyTo = user.UserModel.UserName;
                emailClient.Message = string.Format("<html><body><div> {0} </div> </body></html>", Message);
                emailClient.Subject = "Feedback from " + user.Name + ":" + Subject;
                foreach (var item in temp)
                {
                    emailClient.To = new List<string> { item };
                    emailClient.SendEmail(SmtpServer, SmtpPort, fromEmailID, IsSSL, EmailUserName, EmailPassword);
                }
            }
            catch (Exception)
            {
                
            }
        }
    }
}
