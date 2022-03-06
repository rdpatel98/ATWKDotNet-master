using Firebase.Database;
using ITEK.EMAILHELPER;
using Newtonsoft.Json;
using SHABS.DB;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace SHABS.MODELS
{
    public class UserModel : BaseModel
    {
        private string _userId;
        private string _userName;
        private string _userPassword;
        private string _newPassword;
        private string _userMembershipID;
        private DateTime _lastLoginTime;
        private string _userType;
        private string _userStatus;
        private string _passphrase;

        public string UserId
        {
            get
            {
                return _userId;
            }
            set
            {
                _userId = value;
            }
        }
        public string UserName
        {
            get
            {
                return _userName;
            }
            set
            {
                _userName = value;
            }
        }
        public string UserPassword
        {
            get
            {
                if (_userPassword == null)
                {
                    return string.Empty;
                }
                return _userPassword;
            }
            set
            {
                _userPassword = value;
            }
        }
        public string NewPassword
        {
            get
            {
                return _newPassword;
            }
            set
            {
                _newPassword = value;
            }
        }
        public string UserMemberShipID
        {
            get
            {
                return _userMembershipID;
            }
            set
            {
                _userMembershipID = value;
            }
        }
        public DateTime LastLoginTime
        {
            get
            {
                return _lastLoginTime;
            }
            set
            {
                _lastLoginTime = value;
            }
        }
        public string UserType
        {
            get
            {
                return _userType;
            }
            set
            {
                _userType = value;
            }
        }
        public string UserStatus
        {
            get
            {
                return _userStatus;
            }
            set
            {
                _userStatus = value;
            }
        }
        public string Passphrase
        {
            get
            {
                return _passphrase;
            }
            set
            {
                _passphrase = value;
            }
        }
        public string OldPassword { get; set; }
        public string ConfirmPassword { get; set; }
        public string UserLoginStatus { get; set; }
        public string RegistrationID { get; set; }

        public async Task<string> IsValidUserAsync()
        {
            Error = string.Empty;
            DataTable dt = new DataTable();
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("@username", UserName);
            parameters.Add("@password", UserPassword);
            dt = (new OleDBHelper()).GetData("authenticateUser", parameters);
            if (dt.Rows.Count > 0)
            {
                this.UserStatus = dt.Rows[0]["UserStatus"].ToString();
                if (this.UserStatus != "ACTV")
                {
                    this.UserId = "-1";
                    return "SUCCESS";
                }
                UserId = dt.Rows[0]["userID"].ToString();
                return "SUCCESS";
            }
            else
            {
                return "ERROR";
            }
        }

        public string IsValidPassword()
        {
            //CheckPasswordValidity
            DataTable dt = new DataTable();
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("@userid", UserId);
            parameters.Add("@password", OldPassword);
            dt = (new OleDBHelper()).GetData("CheckPasswordValidity", parameters);
            if (dt.Rows.Count > 0)
            {

                return "SUCCESS";
            }
            else
            {
                return "ERROR";
            }
        }

        public string ChangePassword(int indc = 0)
        {
            if (NewPassword != ConfirmPassword)
            {
                return "Passwords don't match";
            }
            string response = "SUCCESS";
            if (indc == 0)
            {
                response = IsValidPassword();
            }
            if (response == "SUCCESS")
            {
                Dictionary<string, string> param = new Dictionary<string, string>();
                param.Add("@userid", UserId);
                param.Add("@newpassword", NewPassword);
                param.Add("@modifiedBy", ModifierUserId);
                int result = (new OleDBHelper()).InsertUpdateData("ChangePassword", param);
                if (result > 0)
                {
                    return "SUCCESS";
                }
                else
                {
                    return "Sorry Some Error Occured";
                }
            }
            else
            {
                return "Invalid Current Password";
            }
        }

        private static string generateEmail(string password)
        {
            string email = "<html><body><div> Your password is {0} </div> </body></html>";
            email = string.Format(email, password);
            return email;
        }

        public static string SetUserStatus(string userid, string status)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("@userid", userid);
            parameters.Add("@status", status);
            int result = (new OleDBHelper()).InsertUpdateData("MaintainUserLoginStatus", parameters);
            if (result > 0)
            {
                return "SUCCESS";
            }
            else
            {
                return "ERROR";
            }
        }

        public static string ForgotPassword(string identifier)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            DataTable dt = new DataTable();
            parameters.Add("@username", identifier);
            dt = (new OleDBHelper()).GetData("GetPassword", parameters);
            if (dt.Rows.Count > 0)
            {
                string password = dt.Rows[0]["password"].ToString();
                //sendemail here GetPassword
                string SmtpServer = ConfigurationManager.AppSettings["SMTPServer"];
                int SmtpPort = Convert.ToInt32(ConfigurationManager.AppSettings["SMTPPort"]);
                string EmailUserName = ConfigurationManager.AppSettings["EmailUserName"];
                string EmailPassword = ConfigurationManager.AppSettings["EmailPassword"];
                bool IsSSL = ConfigurationManager.AppSettings["EnableSSL"] == "1";
                string fromEmailID = ConfigurationManager.AppSettings["EmailID"];

                EmailMessage emailClient = new EmailMessage();
                emailClient.Message = generateEmail(password);
                emailClient.Subject = "MyApp: Forgot Pasword";
                emailClient.To = new List<string> { identifier };
                emailClient.SendEmail(SmtpServer, SmtpPort, fromEmailID, IsSSL, EmailUserName, EmailPassword);

                return "SUCCESS";
            }
            else
            {
                return "The username does not exist. Please enter a valid username or register.";
            }
        }


    }
}